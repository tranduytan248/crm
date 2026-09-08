using System;

namespace Core.API.Models
{
    public class MemberUpdateInputModel : APIModel
    {
        public string Traveler_ID { get; set; }
        public int Nationlity { get; set; } = 0;
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public int Gender { get; set; } = 0;
    }

    public class MemberInputModel: APIModel
    {
        public string Traveler_ID { get; set; }
    }

    public class MemberPassInputModel : MemberInputModel
    {
        public string OldPass { get; set; } = "";
        public string NewPass { get;set; } = "";
    }

    public class MemberInfoModel
    {
        public string Traveler_ID { get; set; }
        public int Member_ID { get; set; } = 0;
        public string Facebook_ID { get; set; }
        public string Google_ID { get; set; }
        public string EmailTraveler { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public int Gender { get; set; }
        public string AddressTraveler { get; set; } = "";
        public int Nationality { get; set; }
        public string Avatar { get; set; }
        public int IDError { get; set; } = 0;
        public string Account { get;set; }
        public string Accommodation_Location_ID { get; internal set; }
        public string Password { get; set; }
        public string CustomerCode { get; set; }
        public string OTP { get; set; }
    }
}