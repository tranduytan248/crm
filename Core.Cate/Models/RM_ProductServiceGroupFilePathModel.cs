using System;
using System.Collections.Generic;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_ProductServiceGroupFilePathModel : BaseModel
    {
        public int GroupFilePathID { get; set; }
        public int ProductServiceID { get; set; }
        [CustomRequired]
        [CustomDisplayName("ThongBao_TieuDe")]
        public string Title { get; set; }
        [CustomRequired]
        [CustomDisplayName("Menu_Label_Position")]
        public int Position { get; set; }
        //[CustomRequired]
        [CustomDisplayName("Category_Note")]
        public string Note { get; set; }
        public string UserUpdated { get; set; }
        public DateTime DateUpdated { get; set; }
        [CustomRequired]
        [CustomDisplayName("FileName")]
        public string ListFiles { get; set; }
    }
}
