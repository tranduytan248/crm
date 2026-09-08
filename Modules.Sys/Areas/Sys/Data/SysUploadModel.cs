using System.Web;

namespace Modules.Sys.Areas.Sys.Data
{
    public class SysUploadModel
    {
        public string AbsolutePath { get; set; }

        public HttpPostedFileBase FileUpload { get; set; }
    }
}