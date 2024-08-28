using OfficeOpenXml;

namespace BuildingBlocks.Utilities;

public static class ExcelHelper
{
    public static void ExportToExcel<T>(this IEnumerable<T> entities, FileInfo targetFile, string sheet = "Sheet1") where T: class
    {
        using (var excelFile = new ExcelPackage(targetFile))
        {
            var worksheet = excelFile.Workbook.Worksheets.Add(sheet);
            worksheet.Cells["A1"].LoadFromCollection(Collection: entities, PrintHeaders: true);
            excelFile.Save();
        }
    }
}