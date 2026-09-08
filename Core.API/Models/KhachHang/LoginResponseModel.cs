using System.Collections.Generic;

namespace Modules.API.Models.KhachHang
{
    public class LoginResponseModel
    {
        public int UserId { get; set; }             
        public string CustomerCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; } 
        public string Phone { get; set; }
        public List<ContractModel> Contracts { get; set; } = new List<ContractModel>();
    }
    public class ContractModel
    {
        public string ContractCode { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
    }
}
