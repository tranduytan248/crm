using System;
using System.Collections.Generic;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class Cate_CustomerModel : BaseModel
    {
        public int CustomerId { get; set; }
        [CustomDisplayName("UserName")]
        public string Username { get; set; }
        [CustomRequired]
        [CustomDisplayName("FullName_Label")]
        public string FullName { get; set; }
        [CustomRequired]
        [CustomDisplayName("User_Label_Phone")]
        public string Phone { get; set; }
        [CustomDisplayName("Email")]
        public string Email { get; set; }
        [CustomRequired]
        [CustomDisplayName("DiaChi")]
        public string Address { get; set; }
        [CustomRequired]
        [CustomDisplayName("DateOfBirth_Label")]
        public DateTime? DateOfBirth { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class Cate_CustomerSearchModel
    {
        [CustomDisplayName("News_Label_TuKhoa")]
        public string TuKhoa { get; set; }
        [CustomDisplayName("FileManager_Label_CreatedDate")]
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }
    public class Cate_CustomerDuplicateModel
    {
        public string CustomerName { get; set; }
        public string TaxCode { get; set; }
        public string Reason { get; set; }
    }
    public class Cate_CustomerImportResultModel
    {
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public List<Cate_CustomerDuplicateModel> DuplicateRows { get; set; }
    }
}
