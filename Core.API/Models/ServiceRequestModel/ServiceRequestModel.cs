using System;
using System.Runtime.InteropServices;

namespace Modules.API.Models.ServiceRequestModel
{
    public class ServiceRequestModel
    {
        public int RequestId { get; set; }
        public int ContractId { get; set; }
        public string RequestType { get; set; }
        public string RequestDetails { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public string RequesterName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}

