using Modules.API.Models.KhachHang;
using System;
using System.Runtime.InteropServices;

namespace Modules.API.Models.ServiceRequestModel
{
    public class NewConnectionRequestModel
    {
        public string ServiceType { get; set; }
        public string FullNameOrOrganization { get; set; }
        public string DateOfBirth { get; set; } 
        public IdentityModel Identity { get; set; }
        public ContactModel Contact { get; set; }
        public string RegistrationAddress { get; set; }
        public string UsagePurpose { get; set; }

        // Optional: lưu kết quả từ DB
        public string RequestId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }

    public class IdentityModel
    {
        public string Type { get; set; }
        public string Number { get; set; }
        public string IssueDate { get; set; }
        public string IssuedPlace { get; set; } 
    }

    public class ContactModel
    {
        public string PhoneNumber { get; set; } // đổi tên cho đúng JSON
        public string Email { get; set; }
    }

}