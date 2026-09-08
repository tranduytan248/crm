using System.Collections.Generic;

namespace TSFramework.Libs.Consts
{
    public static class ConstMIMEType
    {
        public static string ImageType = "image/";
        public static string ApplicationType = "application/";

        public static Dictionary<string, string> OfficeMIMETypes = new Dictionary<string, string>
        {
            {".doc", "application/msword"},
            {".dot", "application/msword"},
            {".docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {".dotx","application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
            {".docm","application/vnd.ms-word.document.macroEnabled.12"},
            {".dotm","application/vnd.ms-word.template.macroEnabled.12"},
            {".xls", "application/vnd.ms-excel"},
            {".xlt", "application/vnd.ms-excel"},
            {".xla", "application/vnd.ms-excel"},
            {".xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {".xltx","application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
            {".xlsm","application/vnd.ms-excel.sheet.macroEnabled.12"},
            {".xltm","application/vnd.ms-excel.template.macroEnabled.12"},
            {".xlam","application/vnd.ms-excel.addin.macroEnabled.12"},
            {".xlsb","application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
            {".ppt", "application/vnd.ms-powerpoint"},
            {".pot", "application/vnd.ms-powerpoint"},
            {".pps", "application/vnd.ms-powerpoint"},
            {".ppa", "application/vnd.ms-powerpoint"},
            {".pdf", "application/pdf"},
            {".zip", "application/zip"}
    };

        public static Dictionary<string, string> ImageMIMETypes = new Dictionary<string, string>
        {
            { "image/apng",".apng" },
            { "image/avif" ,".avif"},
            { "image/gif",".gif" },
            { "image/png",".png" },
            { "image/jpeg",".jpg,.jpeg,.jfif,.jfif,.pjp" },
            { "image/svg+xml",".svg" },
            { "image/webp",".webp" },
        };

        public static Dictionary<string, string> DocMIMETypes = new Dictionary<string, string>
        {
            {".doc", "application/msword"},
            {".docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {".dotx","application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
            {".docm","application/vnd.ms-word.document.macroEnabled.12"},
            {".dotm","application/vnd.ms-word.template.macroEnabled.12"},
            {".xls", "application/vnd.ms-excel"},
            {".xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {".xlsm","application/vnd.ms-excel.sheet.macroEnabled.12"},
            {".xlsb","application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
            {".ppt", "application/vnd.ms-powerpoint"},
            {".pptx", "application/vnd.ms-powerpoint"},
            {".pot", "application/vnd.ms-powerpoint"},
            {".pps", "application/vnd.ms-powerpoint"},
            {".ppsx", "application/vnd.ms-powerpoint"},
            {".ppam", "application/vnd.ms-powerpoint"},
        };

        public static bool IsImage(string contentType)
        {
            return ImageMIMETypes.ContainsKey(contentType);
        }

        public static bool IsDoc(string fileExt)
        {
            return DocMIMETypes.ContainsKey(fileExt);
        }

        public static bool IsPdf(string fileExt)
        {
            return ".pdf" == fileExt;
        }
    }
}