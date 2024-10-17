﻿using System.Drawing;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BuildingBlocks.Extensions;

public static class ExcelExtensions
{
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