using System;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_RolesModel : BaseSearchModel
    {
        //RoleID 
        public int RoleID { get; set; }
        //RoleName 
        public string RoleName { get; set; }
        //CreatedBy 
        public string CreatedBy { get; set; }
        //CreatedDate 
        public DateTime CreatedDate { get; set; }
        //UpdatedBy 
        public string UpdatedBy { get; set; }
        //UpdatedDate 
        public DateTime UpdatedDate { get; set; }
        //IsDeleted 
        public bool IsDeleted { get; set; }
    }
}
