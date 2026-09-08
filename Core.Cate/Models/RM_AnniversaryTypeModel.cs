using System;
using System.Collections.Generic;
using System.Linq;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_AnniversaryTypeModel : BaseSearchModel
    {
        //AnniversaryType_ID 
        public int AnniversaryType_ID { get; set; }
        //CodeAnniversaryType 
        [CustomRequired]
        [CustomDisplayName("AnniversaryType_Code")]
        public string CodeAnniversaryType { get; set; }
        //NameAnniversaryType 
        [CustomRequired]
        [CustomDisplayName("AnniversaryType_Name")]
        public string NameAnniversaryType { get; set; }
        //IsReminder 
        [CustomDisplayName("AnniversaryType_IsReminder")]
        public bool IsReminder { get; set; }
        //IsDeleted 
        public bool IsDeleted { get; set; }
    }

    public class RM_AnniversaryTypeSearchModel : BaseSearchModel
    {

        [CustomDisplayName("Label_TuKhoa")]
        public string Keyword { get; set; }

        [CustomDisplayName("AnniversaryType_IsReminder")]
        public string IsReminder { get; set; }
    }
}
