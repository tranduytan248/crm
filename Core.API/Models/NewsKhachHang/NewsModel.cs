using System;

namespace Modules.API.Models.NewsKhachHang
{
    public class UsageAlertModel
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}
