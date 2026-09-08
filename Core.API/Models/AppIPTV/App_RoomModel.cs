using System;
 
namespace Core.API.Models
{
    public class App_RoomModel
    {
        public int Room_ID { get; set; }
        public string RCode { get; set; }
        public int Accommodation_Location_ID { get; set; }
        public string RNumber { get; set; }
        public string IPAddress { get; set; }
        public string Port { get; set; }
        public int StatusPort { get; set; }
    }
}
