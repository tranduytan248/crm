using System;
 
namespace Core.API.Models
{
    public class App_FoodModel
    {
        public int GroupMenu_ID { get; set; }
        public string GMName { get; set; }
        public string Logo { get; set; }
        public string FoodName { get; set; }
        public string Introduct { get; set; }
        public double Price { get; set; }
        public string ImageThumb { get; set; }
        public bool IsBestSeller { get; set; }
        public string ImageList { get; set; }
    }
}
