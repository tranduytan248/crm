using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class Cate_ProductServiceModel : BaseSearchModel
    {
        public int ProductServiceID { get; set; } = 0;
        [CustomRequired]
        [CustomDisplayName("RMPS_Label_GroupServiceID")]
        public int GroupServiceID { get; set; }
        public string GroupServiceName { get; set; }
  
        public int? ParentProductID { get; set; } = 0;
        public string ParentProductName { get; set; }
        [CustomRequired]
        [CustomDisplayName("RMPS_Label_NameProduct")]
        public string NameProduct { get; set; } = "";
        [CustomDisplayName("RMPS_Label_CodeProduct")]
        public string CodeProduct { get; set; } = "";
        [CustomDisplayName("RMPS_Label_ShortNameProduct")]
        public string ShortNameProduct { get; set; } = "";
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string DisplayName { get; set; }
        public string NodeType { get; set; }
        public string SortPath { get; set; }
        public int ParentGroupServiceID { get; set; }
        public int Level { get; set; }
        public string NameProductView { get; set; }
        public int gID { get; set; }
        public int pID { get; set; }
        [CustomRequired]
        [CustomDisplayName("RMPS_Label_IsActived")]
        public Boolean IsActived { get; set; } = true;

        public List<Cate_ProductServiceModel> selectListItems { get; set; }
        [CustomDisplayName("RMPS_Label_Note")]
        public string Note { get; set; }
        [CustomDisplayName("File_Attach_Label")]
        public string FileAttach { get; set; }
        public List<HttpPostedFileBase> DinhKemFile { get; set; }
        public List<RM_ProductServiceFilePathModel> ExistingFiles { get; set; }
        public List<int> DeletedFileIds { get; set; }
        public List<RM_ProductServiceFilePathModel> ProductServiceFilePath { get; set; }
    }

    public class Cate_ProductServiceSearchModel
    {
        public string SearchFile { get; set; } = "";
        public string Search { get; set; } = "";
    }
}
