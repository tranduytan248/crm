using System;
 
namespace Core.API.Models
{
    public class App_BookingModel
    {
        public int Booking_ID { get; set; }
        public int Room_ID { get; set; }
        public string NameCustomer { get; set; }
        public string Nation { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string CCCD { get; set; }
        public int Gender { get; set; }
        public string Rental_Purposes { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
    }
}
