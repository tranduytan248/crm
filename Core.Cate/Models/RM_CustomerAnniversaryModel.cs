using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_CustomerAnniversaryModel : BaseModel
    {
        public int CustomerAnniversaryId { get; set; }

        public int CustomerID { get; set; }

        public string CustomerName { get; set; }

        [Required]
        public int AnniversaryTypeId { get; set; }

        public string NameAnniversaryType { get; set; }

        public bool IsReminder { get; set; }

        [Required]
        public DateTime? AnniversaryDate { get; set; }

        public string Note { get; set; }

        public bool IsLunar { get; set; }

        public bool IsDeleted { get; set; }

        public List<SelectListItem> ListAnniversaryType { get; set; } = new List<SelectListItem>();
    }

    public class RM_CustomerAnniversarySearchModel : BaseModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }

    //public class RM_AnniversaryTypeModel
    //{
    //    public int AnniversaryType_ID { get; set; }
    //    public string CodeAnniversaryType { get; set; }
    //    public string NameAnniversaryType { get; set; }
    //    public bool IsReminder { get; set; }
    //}
}