using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Sys.Models.Sys
{
    public class SysNotificationsModel : BaseModel
    {
        public int Notification_ID { get; set; }
        public int LichSuPhieu_ID { get; set; }
        public int User_ID { get; set; }
        public string PhieuID { get; set; }
        public string TenPhieu { get; set; }
        public string TenQuyTrinh { get; set; }
        public string HoTenNguoiLapPhieu { get; set; }
        public DateTime HanHoanThanh { get; set; }
        public bool DaThucHien { get; set; } = true;
        public bool IsRead { get; set; } = true;
        public int Type { get; set; }
        public string LyDo { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
