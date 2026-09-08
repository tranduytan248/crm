using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Sys.Models.Sys
{
    public class SysMessageModel : BaseModel
    {
        [CustomRequired]
        [CustomDisplayName("Message_Label_LangCode")]
        public string LangCode { get; set; }

        [CustomRequired]
        [CustomDisplayName("Message_Label_LabelKey")]
        public string LabelKey { get; set; }

        [AllowHtml]
        [CustomRequired]
        [CustomDisplayName("Message_Label_Message")]
        public string Message { get; set; }

        public new int? TotalRow { get; set; } = 0;
    }
}