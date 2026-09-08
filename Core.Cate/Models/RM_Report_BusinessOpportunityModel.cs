using System.Collections.Generic;
using System.Web.Mvc;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_Report_BusinessOpportunityModel : BaseModel
    {
        /// <summary>
        /// Loại bản ghi: Project (dự án) hoặc BusinessOpportunity (cơ hội kinh doanh).
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Mã dự án hoặc mã cơ hội kinh doanh tương ứng với DataType, dùng để mở màn hình chi tiết.
        /// </summary>
        public int ObjectID { get; set; }

        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTypeName { get; set; }
        public string CustomerGroupName { get; set; }
        public string ProductServiceNames { get; set; }
        public string ProjectName { get; set; }
        public string ProjectTypeName { get; set; }
        public string OpportunityStatusName { get; set; }
        public string ContactPersonInfo { get; set; }
        public decimal TotalExpectedValue { get; set; }
        public decimal TotalVNPTValue { get; set; }
        public string ExecutionTime { get; set; }
        public string Note { get; set; }
    }

    public class RM_Report_BusinessOpportunitySearchModel
    {
        public int Nam { get; set; }

        /// <summary>
        /// Lọc theo loại bản ghi: rỗng (tất cả), Project hoặc BusinessOpportunity.
        /// </summary>
        public string Loai { get; set; }

        public string TuKhoa { get; set; }

        /// <summary>
        /// Lọc theo loại khách hàng.
        /// </summary>
        public int? CustomerTypeID { get; set; }

        /// <summary>
        /// Lọc theo sản phẩm dịch vụ.
        /// </summary>
        public int? ProductServiceID { get; set; }

        /// <summary>
        /// Lọc theo loại dự án; cơ hội kinh doanh không có tiêu chí này nên sẽ bị loại khi lọc.
        /// </summary>
        public int? ProjectTypeID { get; set; }

        /// <summary>
        /// Lọc theo trạng thái của dự án hoặc cơ hội kinh doanh.
        /// </summary>
        public int? StatusID { get; set; }

        /// <summary>
        /// Danh sách loại khách hàng đổ vào dropdown lọc.
        /// </summary>
        public List<SelectListItem> ListCustomerType { get; set; }

        /// <summary>
        /// Danh sách sản phẩm dịch vụ đổ vào dropdown lọc.
        /// </summary>
        public List<Cate_ProductServiceModel> ListProductService { get; set; }

        /// <summary>
        /// Danh sách loại dự án đổ vào dropdown lọc.
        /// </summary>
        public List<SelectListItem> ListProjectType { get; set; }

        /// <summary>
        /// Danh sách trạng thái dự án đổ vào dropdown lọc.
        /// </summary>
        public List<SelectListItem> ListProjectStatus { get; set; }

        /// <summary>
        /// Danh sách trạng thái cơ hội kinh doanh đổ vào dropdown lọc.
        /// </summary>
        public List<SelectListItem> ListOpportunityStatus { get; set; }
    }
}
