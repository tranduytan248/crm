using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class MN_EmployeeModel : BaseSearchModel
    {
        public int Employee_ID { get; set; }
        public string Employee_Code { get; set; }
        public string FullName { get; set; }
        public int BoPhan_ID { get; set; }
        public int ChucVu_ID { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSaleMember { get; set; }
        public string TenBoPhan { get; set; }
        public string Phone { get; set; }
    }
}