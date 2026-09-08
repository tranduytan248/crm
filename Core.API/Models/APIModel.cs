using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using System;

namespace Core.API.Models
{
    public class APIModel
    {
        public string LangCode { get; set; } = "vi";

    }

    public class InputSearchListModel
    {
        public string Lat { get; set; } = "";
        public string Lng { get; set; } = "";
        public int TypeSort { get; set; } = 1; //1 - Theo rate, 2 - Theo location
        public string SearchContent { get; set; } = "";
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;

    }

    public class InputSearchPlanModel
    {
        public int NumDay { get; set; } = 0; //Số ngày muốn đi du lịch
        public int NumPeople { get; set; } = 0; //Số người đi du lịch
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;

    }

    public class InputSearchNotificationModel
    {
        public string Traveler_ID { get; set; } = "";
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;

    }

    public class InputDataTVModel
    {
        public int ID { get; set; } = 0;
        public string MA { get; set; } = "";

    }

    public class InputDataDeviceModel
    {
        public int Room_ID { get; set; } = 0;
        public string MA { get; set; } = "";
        public string BranchDevice { get; set; } = "";

    }
}