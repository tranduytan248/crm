using System;

namespace Modules.API.Models.NewsKhachHang
{
    public class CustomerContractsModel
    {
        public int Id { get; set; }
        public string ContractCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
    }
}
