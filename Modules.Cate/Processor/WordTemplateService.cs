using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlPowerTools;
using System.Xml.Linq;
using DocumentFormat.OpenXml;
using System.Xml;

public class WordTemplateService
{
    // Thay placeholder trong document (format placeholder: {{Key}})
    public static void ReplacePlaceholdersInDocx(Stream docxStream, Dictionary<string, string> values)
    {
        // document must be seekable
        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(docxStream, true))
        {
            var doc = wordDoc.MainDocumentPart.Document;
            // Lấy tất cả Text elements (có thể split across runs)
            var texts = wordDoc.MainDocumentPart.Document.Descendants<Text>().ToList();

            foreach (var para in wordDoc.MainDocumentPart.Document.Descendants<Paragraph>())
            {
                // lấy tất cả Run->Text trong paragraph
                var runs = para.Elements<Run>().ToList();
                if (runs.Count == 0) continue;

                // build full text and keep mapping run index->text lengths
                var sb = new StringBuilder();
                var runTextList = new List<string>();
                foreach (var r in runs)
                {
                    var t = r.GetFirstChild<Text>()?.Text ?? "";
                    runTextList.Add(t);
                    sb.Append(t);
                }

                var full = sb.ToString();
                if (string.IsNullOrEmpty(full)) continue;

                // Nếu có placeholder nào trong full
                bool hasAny = values.Keys.Any(k => full.Contains("{{" + k + "}}"));
                if (!hasAny) continue;

                // Thay thế trên chuỗi đầy đủ
                foreach (var kv in values)
                {
                    string placeholder = "{{" + kv.Key + "}}";
                    if (full.Contains(placeholder))
                        full = full.Replace(placeholder, kv.Value ?? "");
                }

                // Xóa tất cả run cũ trong paragraph và tạo một run mới giữ nguyên định dạng cơ bản của run đầu
                var firstRun = runs.First();
                var newRun = new Run(firstRun.RunProperties?.CloneNode(true) as RunProperties, new Text(full) { Space = SpaceProcessingModeValues.Preserve });

                // remove old runs and append newRun
                foreach (var r in runs) r.Remove();
                para.AppendChild(newRun);
            }

            doc.Save();
        }
    }



    // Convert docx Stream -> HTML string (sử dụng OpenXmlPowerTools)
    public static string ConvertDocxStreamToHtmlString(Stream docxStream, string imageDirForHtml = null)
    {
        // OpenXmlPowerTools expects a WmlDocument
        docxStream.Position = 0;
        var memory = new MemoryStream();
        docxStream.CopyTo(memory);
        memory.Position = 0;

        var wml = new WmlDocument("temp.docx", memory.ToArray());

        var settings = new HtmlConverterSettings()
        {
            FabricateCssClasses = true,
            CssClassPrefix = "docx-",
            PageTitle = "Converted from docx"
        };

        // Convert -> XElement (HTML)
        var htmlElement = HtmlConverter.ConvertToHtml(wml, settings);

        // XDocument -> string
        var htmlDoc = new XDocument(
            new XDocumentType("html", null, null, null),
            htmlElement
        );

        return htmlDoc.ToString();
    }

    // Ví dụ API usage: nhận templatePath, data, trả về html string và lưu file kết quả
    public static string FillTemplateAndExport(string templatePath, Dictionary<string, string> data, string outDocxPath = null, string outHtmlPath = null)
    {
        using (var fs = File.Open(templatePath, FileMode.Open, FileAccess.Read))
        using (var ms = new MemoryStream())
        {
            fs.CopyTo(ms);
            ms.Position = 0;

            // Thay placeholder
            ReplacePlaceholdersInDocx(ms, data);

            // Nếu muốn lưu docx kết quả ra đĩa
            ms.Position = 0;
            if (!string.IsNullOrEmpty(outDocxPath))
            {
                File.WriteAllBytes(outDocxPath, ms.ToArray());
            }

            // Convert to HTML string
            ms.Position = 0;
            string html = ConvertDocxStreamToHtmlString(ms);

            // Lưu html ra file nếu cần
            if (!string.IsNullOrEmpty(outHtmlPath))
            {
                File.WriteAllText(outHtmlPath, html, Encoding.UTF8);
            }

            return html;
        }
    }
}
