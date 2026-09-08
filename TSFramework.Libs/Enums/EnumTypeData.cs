using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFramework.Libs.Enums
{
    public enum EnumTypeData
    {
        // Cơ sở lưu trú
        [Description("Accommodations")] Accommodations,
        // Điểm vui chơi
        [Description("EntertainmentSpots")] EntertainmentSpots,
        // Tin tức
        [Description("News")] News,
        // Nhà hàng
        [Description("Restaurants")] Restaurants, 
        // Điểm mua sắm
        [Description("ShoppingSpots")] ShoppingSpots,
        // Đơn vị lữ hành
        [Description("TravelUnits")] TravelUnits,
        // Điểm du lịch
        [Description("TouristDestinations")] TouristDestinations,
    }
}
