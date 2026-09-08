using System;
using System.Collections.Generic;
using System.Linq;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class NhanVienDto
    {
        public int NhanVien_ID { get; set; }
        public string Ma_NV { get; set; }
        public string Ho_NV { get; set; }
        public string Ten_NV { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string DienThoai_DD { get; set; }
        public string DienThoai_CD { get; set; }
        public bool Gioi_Tinh { get; set; }
        public string Email { get; set; }
        public string Mat_Khau { get; set; }
        public int DonVi_ID { get; set; }
        public int ChucDanh_ID { get; set; }
        public DateTime? Ngay_Tao { get; set; }
        public DateTime? Ngay_CN { get; set; }
        public string Nguoi_Tao { get; set; }
        public string Nguoi_CN { get; set; }
        public bool Da_Xoa { get; set; }
        public decimal? He_So_HS3P { get; set; }
        public int CapQuanLy { get; set; }
        public bool HoatDong { get; set; }
        public int? LevelManager { get; set; }
    }
    public class BoPhanDto
    {
        public int DonVi_ID { get; set; }
        public string Ma_DV { get; set; }
        public string Ten_DV { get; set; }
        public int DonVi_Cha_ID { get; set; }
        public DateTime? Ngay_Tao { get; set; }
        public DateTime? Ngay_CN { get; set; }
        public string Nguoi_Tao { get; set; }
        public string Nguoi_CN { get; set; }
        public bool Da_Xoa { get; set; }
        public string Ma_DV_BCN { get; set; }
        public int CapQuanLy { get; set; }
    }

    public class ChucVuDto
    {
        public int ChucDanh_ID { get; set; }
        public string Ma_CD { get; set; }
        public string Ten_CD { get; set; }
        public bool Lanh_Dao { get; set; }
        public DateTime? Ngay_Tao { get; set; }
        public DateTime? Ngay_CN { get; set; }
    }
    public class ApiResponse<T>
    {
        public List<T> Data { get; set; }
    }

}
