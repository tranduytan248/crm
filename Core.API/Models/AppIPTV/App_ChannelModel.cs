using System;
 
namespace Core.API.Models
{
    public class App_ChannelModel
    {
        public int Channel_ID { get; set; }
        public string CName { get; set; }
        public string Type { get; set; }
        public string Link { get; set; }
        public string Logo { get; set; }
        public string RemoteButton { get; set; }
        public string IPAddress { get; set; }
        public int GroupChannel_ID { get; set; }
        public string GC_Code { get; set; }
        public string GC_Name { get; set; }
        public string LogoGroup { get; set; }
    }
}
