using System;
using System.ComponentModel;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    /// <summary>
    /// Thông tin tham số của mẫu email.
    /// </summary>
    public class SysMailTemplateParamModel : BaseModel
    {
        #region Property

        public int MailTemplateParamId { get; set; }

        public int MailTemplateId { get; set; }

        [DisplayName("Mã tham số")]
        public string ParamCode { get; set; }

        [DisplayName("Tên tham số")]
        public string ParamName { get; set; }

        [DisplayName("Bắt buộc")]
        public bool IsRequired { get; set; }

        [DisplayName("Giá trị mặc định")]
        public string DefaultValue { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        #endregion
    }
}
