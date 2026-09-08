using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

public static class ExcelExportHelper
{
    /// <summary>
    /// Hàm export excel dùng chung
    /// </summary>
    public static FileStreamResult Export<T>(
        string templatePath,
        string sheetName,
        string headerCell,
        string headerText,
        int startRow,
        int totalColumns,
        List<T> data,
        Action<ExcelWorksheet, int, T> rowRenderer,
        string outputFileName)
    {
        string urlFile = HostingEnvironment.MapPath(templatePath);
        var package = new ExcelPackage(new FileInfo(urlFile));
        var ws = package.Workbook.Worksheets[sheetName];

        // Set header
         if(!string.IsNullOrEmpty(headerText))
        ws.Cells[headerCell].Value = headerText;

        // Render từng dòng dữ liệu
        for (int i = 0; i < data.Count; i++)
        {
            int row = startRow + i;

            // Hàm render dòng do Controller truyền vào
            rowRenderer(ws, row, data[i]);

            // Border
            var range = ws.Cells[row, 1, row, totalColumns];
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        // Xuất file
        var fileStream = new MemoryStream();
        package.SaveAs(fileStream);
        fileStream.Position = 0;

        return new FileStreamResult(fileStream,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = outputFileName
        };
    }
}
