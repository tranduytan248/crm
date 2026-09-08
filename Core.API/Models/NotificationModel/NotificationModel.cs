using System;

namespace Modules.API.Models.NotificationModel
{
    public class NotificationModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
