using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_CustomerSearchModel : BaseSearchModel
    {
        public RM_CustomerSearchModel()
        {
            ListCustomerType = new List<SelectListItem>();
            ListStatus = new List<SelectListItem>();
        }
        public string Keyword { get; set; }
        [CustomDisplayName("CustomerType_Title")]
        public int? CustomerTypeID { get; set; }
        [CustomDisplayName("Customer_Status_Label")]
        public int? CustomerStatusID { get; set; }
        public List<SelectListItem> ListCustomerType { get; set; }
        public List<SelectListItem> ListStatus { get; set; }
    }
}
