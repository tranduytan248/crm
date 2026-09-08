using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Cate.Caches;
using Core.Cate.Models;
using Newtonsoft.Json;

namespace Core.Cate.Services
{
    /// <summary>
    /// Service đồng bộ dữ liệu user, donvi, chucvu.
    /// </summary>
    public class SyncDataService
    {
        private readonly MN_BoPhanCache _boPhanCache = new MN_BoPhanCache();
        private readonly MN_ChucVuCache _chucVuCache = new MN_ChucVuCache();

        #region === PUBLIC METHODS ===

        public async Task<DataTable> GetBoPhanAsync()
        {
            var url = ConfigurationManager.AppSettings["Key_Get_Department_API"];
            return await GetDataTableAsync<BoPhanDto>(url, BuildBoPhanTable);
        }

        public async Task<DataTable> GetChucVuAsync()
        {
            var url = ConfigurationManager.AppSettings["Key_Get_Position_API"];
            return await GetDataTableAsync<ChucVuDto>(url, BuildChucVuTable);
        }

        public async Task<DataTable> GetNhanVienAsync()
        {
            var url = ConfigurationManager.AppSettings["Key_Get_User_API"];
            return await GetDataTableAsync<NhanVienDto>(url, BuildNhanVienTable);
        }

        #endregion

        #region === CORE GENERIC ===

        private async Task<DataTable> GetDataTableAsync<T>(
            string url,
            Func<List<T>, DataTable> builder)
        {
            using (var http = new HttpClient())
            {
                var json = await http.GetStringAsync(url);
                var obj = JsonConvert.DeserializeObject<ApiResponse<T>>(json);

                return builder(obj.Data);
            }
        }

        #endregion

        #region === BUILD TABLE ===

        private DataTable BuildBoPhanTable(List<BoPhanDto> data)
        {
            var table = new DataTable();

            table.Columns.Add("DonVi_ID", typeof(int));
            table.Columns.Add("Ma_DV", typeof(string));
            table.Columns.Add("Ten_DV", typeof(string));
            table.Columns.Add("DonVi_Cha_ID", typeof(int));
            table.Columns.Add("Ngay_Tao", typeof(DateTime));
            table.Columns.Add("Ngay_CN", typeof(DateTime));
            table.Columns.Add("Nguoi_Tao", typeof(string));
            table.Columns.Add("Nguoi_CN", typeof(string));
            table.Columns.Add("Da_Xoa", typeof(bool));
            table.Columns.Add("Ma_DV_BCN", typeof(string));
            table.Columns.Add("CapQuanLy", typeof(int));

            foreach (var item in data)
            {
                table.Rows.Add(
                    item.DonVi_ID,
                    item.Ma_DV,
                    item.Ten_DV,
                    item.DonVi_Cha_ID == 0 ? (object)DBNull.Value : item.DonVi_Cha_ID,
                    item.Ngay_Tao ?? (object)DBNull.Value,
                    item.Ngay_CN ?? (object)DBNull.Value,
                    item.Nguoi_Tao,
                    item.Nguoi_CN,
                    item.Da_Xoa,
                    item.Ma_DV_BCN,
                    item.CapQuanLy
                );
            }

            return table;
        }

        private DataTable BuildChucVuTable(List<ChucVuDto> data)
        {
            var table = new DataTable();

            table.Columns.Add("ChucDanh_ID", typeof(int));
            table.Columns.Add("Ma_CD", typeof(string));
            table.Columns.Add("Ten_CD", typeof(string));
            table.Columns.Add("Lanh_Dao", typeof(bool));
            table.Columns.Add("Ngay_Tao", typeof(DateTime));
            table.Columns.Add("Ngay_CN", typeof(DateTime));

            foreach (var item in data)
            {
                table.Rows.Add(
                    item.ChucDanh_ID,
                    item.Ma_CD,
                    item.Ten_CD,
                    item.Lanh_Dao,
                    item.Ngay_Tao ?? (object)DBNull.Value,
                    item.Ngay_CN ?? (object)DBNull.Value
                );
            }

            return table;
        }

        private DataTable BuildNhanVienTable(List<NhanVienDto> data)
        {
            var table = new DataTable();

            table.Columns.Add("Email");
            table.Columns.Add("UserName");
            table.Columns.Add("MaNV");
            table.Columns.Add("FullName");
            table.Columns.Add("Phone");
            table.Columns.Add("MaChucVu");
            table.Columns.Add("MaBoPhan");
            table.Columns.Add("IsActive", typeof(bool));
            table.Columns.Add("IsDeleted", typeof(bool));
            table.Columns.Add("CreatedDate", typeof(DateTime));
            table.Columns.Add("CreatedBy");

            var boPhanMap = _boPhanCache.GetAll()
                .ToDictionary(x => x.BoPhan_ID, x => x.MaBoPhan);

            var chucVuMap = _chucVuCache.GetAll()
                .ToDictionary(x => x.ChucVu_ID, x => x.MaChucVu);

            foreach (var item in data)
            {
                var email = item.Email;
                if (string.IsNullOrEmpty(email)) continue;

                var userName = email.Split('@')[0];

                var maBoPhan = boPhanMap.ContainsKey(item.DonVi_ID)
                    ? boPhanMap[item.DonVi_ID]
                    : null;

                var maChucVu = chucVuMap.ContainsKey(item.ChucDanh_ID)
                    ? chucVuMap[item.ChucDanh_ID]
                    : null;

                table.Rows.Add(
                    email,
                    userName,
                    item.Ma_NV,
                    $"{item.Ho_NV} {item.Ten_NV}",
                    item.DienThoai_DD,
                    maChucVu,
                    maBoPhan,
                    item.HoatDong,
                    item.Da_Xoa,
                    item.Ngay_Tao,
                    item.Nguoi_Tao
                );
            }

            return table;
        }

        #endregion
    }
}
