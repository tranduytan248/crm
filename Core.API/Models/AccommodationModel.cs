using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Log.Models
{
    public class AccommodationModel
    {
        public string Name { get; set; } = "";
        public string Latitude { get; set; } = "";
        public string Longitude { get; set; } = "";
        public double NumRate { get; set; } = 0.0;
        public double Distance { get; set; } = 0.0;
        public string ImgThumb { get; set; } = "";
        public string PlaceInfoId { get; set; } = "";
        public int RowNumber { get; set; } = 0;
    }

    public class DataAccommodationModel
    {
        public List<AccommodationModel> data { get; set; }
        public int RowNumber { get; set; } = 0;
    }

    public class AccommodationDetailModel
    {
        public string Name { get; set; } = "";
        public string Latitude { get; set; } = "";
        public string Longitude { get; set; } = "";
        public double NumRate { get; set; } = 0.0;
        public string ImgThumb { get; set; } = "";
        public string PlaceInfoId { get; set; } = "";
        public string Images { get; set; } = "";
        public string Description { get; set; } = "";
        public string BaseURL { get; set; } = "";
        public string Address { get; set; } = "";

        public List<RatingAccommodationModel> Reviews { get; set; } = new List<RatingAccommodationModel>();
        public List<RoomAccommodationModel> Rooms { get; set; } = new List<RoomAccommodationModel>();
    }

    public class RatingAccommodationModel
    {
        public double PointRate { get; set; } = 0.0;
        public string TitleReview { get; set; } = "";
        public string ContentReview { get; set; } = "";
        public string DateReview { get; set; } = "";
        public string Avatar { get; set; } = "";
        public string FullName { get; set; } = "";
        public string UserFeedback { get; set; } = "";
        public string DateFeedback { get; set; } = "";
        public string ContentFeedback { get; set; } = "";
        public string FullNameUserFeedback { get; set; } = "";
        public string URLImage { get; set; } = "";
    }

    public class RoomAccommodationModel
    {
        public string RoomName { get; set; } = string.Empty;
        public string TypeRoomName { get; set; } = string.Empty;
        public double Price { get; set; } = 0.0;
        public int NumberOfGuest { get; set; } = 0;
    }
}
