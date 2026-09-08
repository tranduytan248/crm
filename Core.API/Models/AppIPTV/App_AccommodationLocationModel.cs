using System;
 
namespace Core.API.Models
{
    public class App_AccommodationLocationModel 
    {
       public int Accommodation_Location_ID { get; set; }
       public string AL_Code { get; set; }
       public string AL_Name { get; set; }
       public string Phone { get; set; }
       public string MST { get; set; }
       public string Logo { get; set; }
       public string VideoIntro { get; set; }
       public bool IsActived { get; set; }
       public DateTime CreatedDate { get; set; }
       public string CreatedBy { get; set; }
       public DateTime UpdatedDate { get; set; }
       public string UpdatedBy { get; set; }
       public bool IsDeteled { get; set; }
       public string Account { get; set; }
       public string KeyRandom { get; set; }
       public string KeyValid { get; set; }
       public int Package_ID { get; set; }
       public string RTP_IP_Address { get; set; }
       public string IntroSupport { get; set; }
       public string Unicast_IP_Address { get; set; }
    }
}
