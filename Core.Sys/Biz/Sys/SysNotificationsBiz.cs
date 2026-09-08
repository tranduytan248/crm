using Core.Sys.Models.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Sys.Biz.Sys
{
    public class SysNotificationsBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";

        private readonly string _ThongBao_Get = "Sys_Notifications_Get";
        private readonly string _ThongBao_GetByID = "Sys_Notifications_GetById";
        private readonly string _ThongBao_Save = "Sys_Notifications_Save";
        private readonly string _ThongBao_Delete = "Sys_Notifications_Delete";
        private readonly string _Notifications_GetByUser = "Sys_Notifications_GetByUser";
        private readonly string _Notifications_MarkAsRead = "Sys_Notifications_MarkAsRead";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns></returns>
        private List<SysNotificationsModel> LoadList(out int total, BaseSearchModel search, string username)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<SysNotificationsModel>(_Notifications_GetByUser,
                DATA_PROVIDER_NAME,
                username,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách thông tin theo giá trị lọc
        /// </summary>
        /// <returns></returns>
        public List<SysNotificationsModel> GetByUser(string username, out int total, BaseSearchModel search = null)
        {
            var data = LoadList(out total, search, username);
            return data;
        }

        /// <summary>
        /// Đánh dấu đã đọc thông báo
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int MarkAsRead(int id, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_Notifications_MarkAsRead, DATA_PROVIDER_NAME, id, username);
            return result.GetValueOrDefault(0);
        }
    }
}
