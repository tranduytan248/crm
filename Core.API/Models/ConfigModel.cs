using System;

namespace Core.API.Models
{
    public class ConfigModel
    {
        public int Config_ID { get; set; }
        public string ImageUrl { get; set; }
        public string KeyConfig { get; set; }
        public string LanguageCode { get; set; }
        public string ContentConfig { get; set; }

    }
}