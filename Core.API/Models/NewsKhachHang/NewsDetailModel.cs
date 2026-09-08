using System;

namespace Modules.API.Models.NewsKhachHang
{
    public class NewsDetailModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}
