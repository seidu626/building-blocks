using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BuildingBlocks.Extensions;

public static class ExcelExtensions
{
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

    private static void ApplyRowFormatting(ExcelWorksheet worksheet, IReadOnlyCollection<object> entities, Color[] rowColors)
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