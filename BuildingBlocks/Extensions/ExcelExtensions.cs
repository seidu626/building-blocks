using System.Drawing;
using System.Reflection;
using BuildingBlocks.Types;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BuildingBlocks.Extensions;

public static class ExcelExtensions
{
    public static List<T> LoadFromExcel<T>(string filePath) where T : new()
    {
        using var package = new ExcelPackage(new FileInfo(filePath));
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null)
            throw new InvalidOperationException("No worksheet found in the Excel file.");

        return ParseWorksheet<T>(worksheet);
    }

    // New method: Load Excel file from a MemoryStream
    public static List<T> LoadFromStream<T>(MemoryStream stream) where T : new()
    {
        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null)
            throw new InvalidOperationException("No worksheet found in the Excel file.");

        return ParseWorksheet<T>(worksheet);
    }

    // Shared logic to parse the worksheet and map rows to objects
    private static List<T> ParseWorksheet<T>(ExcelWorksheet worksheet) where T : new()
    {
        var result = new List<T>();
        var start = worksheet.Dimension.Start;
        var end = worksheet.Dimension.End;

        var headerMap = GetHeaderMap<T>(worksheet, start.Row);

        for (int row = start.Row + 1; row <= end.Row; row++) // Skip header row
        {
            var entity = new T();
            bool isRowEmpty = true;

            foreach (var header in headerMap)
            {
                var cellValue = worksheet.Cells[row, header.Column].Text;

                if (string.IsNullOrWhiteSpace(cellValue)) continue; // Skip empty cells
                isRowEmpty = false; // Mark the row as not empty

                SetPropertyValue(entity, header.Property, cellValue);
            }

            if (!isRowEmpty) // Add only non-empty rows
                result.Add(entity);
        }

        return result;
    }

    // Map Excel headers to the corresponding properties using attributes
    private static List<(PropertyInfo Property, int Column)> GetHeaderMap<T>(ExcelWorksheet worksheet, int headerRow)
    {
        var properties = typeof(T).GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(ExcelHeaderAttribute)))
            .Select(p => new
            {
                Property = p,
                Header = p.GetCustomAttribute<ExcelHeaderAttribute>()?.Name
            }).ToList();

        var headerMap = new List<(PropertyInfo Property, int Column)>();

        for (int col = worksheet.Dimension.Start.Column; col <= worksheet.Dimension.End.Column; col++)
        {
            var headerText = worksheet.Cells[headerRow, col].Text.Trim();

            var matchingProperty = properties.FirstOrDefault(p =>
                string.Equals(p.Header, headerText, StringComparison.OrdinalIgnoreCase));

            if (matchingProperty != null)
            {
                headerMap.Add((matchingProperty.Property, col));
            }
        }

        return headerMap;
    }

    // Set the value of a property, handling type conversion
    private static void SetPropertyValue<T>(T entity, PropertyInfo property, string value)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                property.SetValue(entity, null);
                return;
            }

            var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var convertedValue = Convert.ChangeType(value, targetType);
            property.SetValue(entity, convertedValue);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to convert '{value}' to {property.PropertyType.Name}: {ex.Message}");
        }
    }

    public static IEnumerable<T> ConvertTableToObjects<T, TCallerType>(this ExcelWorksheet worksheet,
        ILogger logger) where T : new()
    {
        if (worksheet.Dimension == null)
        {
            throw new InvalidOperationException("Worksheet has no data.");
        }

        var convertDateTime = new Func<double, DateTime>(excelDate =>
        {
            if (excelDate < 1.0)
                throw new ArgumentException("Excel dates cannot be smaller than 0.");

            var dateTime = new DateTime(1900, 1, 1);
            excelDate = excelDate > 60.0 ? excelDate - 2.0 : excelDate - 1.0;
            return dateTime.AddDays(excelDate);
        });

        var tprops = typeof(T).GetProperties()
            .ToDictionary(p => p.Name.ToLower(), p => p);

        var start = worksheet.Dimension.Start;
        var end = worksheet.Dimension.End;
        var rows = worksheet.Cells[start.Row, start.Column, end.Row, end.Column]
            .GroupBy(cell => cell.Start.Row)
            .ToList();

        var headerCells = rows.First().ToList();
        var columns = headerCells
            .Select((hcell, idx) => new { Name = hcell.Text.Trim().ToLower(), Index = idx })
            .Where(c => tprops.ContainsKey(c.Name))
            .ToList();

        return rows.Skip(1)
            .Select(row =>
            {
                var obj = new T();
                foreach (var col in columns)
                {
                    // Check if the column index is within the bounds of the current row.
                    if (col.Index >= row.Count())
                    {
                        logger.LogWarning(
                            $"Skipping row {rows.IndexOf(row)}: Column index {col.Index} out of range for row.");
                        continue;
                    }

                    var cell = row.ElementAt(col.Index);
                    var value = cell.Text;

                    if (string.IsNullOrWhiteSpace(value)) continue;

                    var property = tprops[col.Name];
                    try
                    {
                        if (property.PropertyType == typeof(DateTime))
                        {
                            property.SetValue(obj, convertDateTime(double.Parse(value)));
                        }
                        else
                        {
                            var convertedValue = Convert.ChangeType(value, property.PropertyType);
                            property.SetValue(obj, convertedValue);
                        }
                    }
                    catch
                    {
                        logger.LogWarning($"Failed to convert value '{value}' to type {property.PropertyType}");
                    }
                }

                return obj;
            }).ToList();
    }

    public static void ExportToExcel<T>(
        this IReadOnlyCollection<T> entities,
        FileInfo targetFile,
        string sheetName = "Sheet1",
        Color[] rowColors = null,
        string title = "",
        string author = "",
        string company = "")
        where T : class
    {
        using var excelPackage = new ExcelPackage(targetFile);
        var worksheet = excelPackage.Workbook.Worksheets.Add(sheetName);
        worksheet.TabColor = Color.Blue;
        worksheet.DefaultRowHeight = 12;
        worksheet.HeaderFooter.FirstFooter.LeftAlignedText = $"Generated: {DateTime.Now.ToShortDateString()}";
        worksheet.Cells["A1"].LoadFromCollection(entities, true);

        FormatHeader(worksheet, entities.Count);
        ApplyRowFormatting(worksheet, entities, rowColors);

        excelPackage.Workbook.Properties.Title = title;
        excelPackage.Workbook.Properties.Author = author;
        excelPackage.Workbook.Properties.Company = company;

        excelPackage.Save();
    }

    private static void FormatHeader(ExcelWorksheet worksheet, int columnCount)
    {
        using (var cell = worksheet.Cells[1, 1, 1, columnCount])
        {
            cell.Style.Font.Bold = true;
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.Black);
            cell.Style.Font.Color.SetColor(Color.WhiteSmoke);
            cell.Style.ShrinkToFit = false;
        }
    }

    private static void ApplyRowFormatting(ExcelWorksheet worksheet, IReadOnlyCollection<object> entities,
        Color[] rowColors)
    {
        int rowIndex = 2;
        foreach (var entity in entities)
        {
            if (rowIndex - 2 < rowColors?.Length)
            {
                using (var cell = worksheet.Cells[rowIndex, 1, rowIndex, entities.Count])
                {
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(rowColors[rowIndex - 2]);
                    cell.Style.Font.Color.SetColor(Color.Black);
                }
            }

            rowIndex++;
        }
    }
}