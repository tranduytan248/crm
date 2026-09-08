using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    /// <summary>
    /// Thông tin mẫu email dùng cho cả runtime và màn hình quản trị.
    /// </summary>
    public class SysMailTemplateModel : BaseModel
    {
        #region Property

        public int MailTemplateId { get; set; }

        [DisplayName("Mã mẫu email")]
        public string TemplateCode { get; set; }

        [DisplayName("Tên mẫu email")]
        public string TemplateName { get; set; }

        [DisplayName("Tiêu đề email")]
        public string SubjectTemplate { get; set; }

        [DisplayName("Đường dẫn file")]
        public string FilePath { get; set; }

        [DisplayName("Hoạt động")]
        public bool IsActive { get; set; }

        [DisplayName("Mô tả")]
        public string Description { get; set; }

        [DisplayName("Nội dung mẫu email")]
        [AllowHtml]
        public string TemplateContent { get; set; }

        [DisplayName("File mẫu email")]
        public HttpPostedFileBase TemplateFile { get; set; }

        public List<SysMailTemplateParamModel> TemplateParams { get; set; } = new List<SysMailTemplateParamModel>();

        public string DetectedParamCodes { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        #endregion
    }
}
