using Core.API.Models;
using Core.Cate.Biz;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Modules.API.Models.KhachHang;
using Modules.API.Models.NewsKhachHang;
using Modules.API.Models.ReadingScheduleModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TSFramework.Libs.Processors;

namespace Core.API.Biz.KHACHHANG
{
    /// <summary>
    /// Business logic cho lịch ghi chỉ số đồng hồ nước
    /// </summary>
    public class ReadingScheduleBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _API_KhachHang_GetReadingSchedule = "API_KhachHang_GetReadingSchedule";

        /// <summary>
        /// Lấy lịch ghi chỉ số theo khoảng thời gian
        /// </summary>
        /// <param name="contractId">ID hợp đồng</param>
        /// <param name="fromDate">Từ ngày (string format yyyy-MM-dd)</param>
        /// <param name="toDate">Đến ngày (string format yyyy-MM-dd)</param>
        /// <returns>Danh sách lịch ghi chỉ số</returns>
        public List<ReadingScheduleModel> GetReadingSchedule(int contractId, string fromDate = null, string toDate = null)
        {
            // Truyền trực tiếp string parameters, giống như NewsBiz
            return AppProcessor.ProcedureProvider.ExecuteTypedList<ReadingScheduleModel>(
                _API_KhachHang_GetReadingSchedule,
                DATA_PROVIDER_NAME,
                contractId,
                fromDate,
                toDate
            );
        }

        /// <summary>
        /// Lấy lịch ghi chỉ số 6 tháng gần nhất
        /// </summary>
        /// <param name="contractId">ID hợp đồng</param>
        /// <returns>Danh sách lịch ghi chỉ số</returns>
        public List<ReadingScheduleModel> GetReadingSchedule(int contractId)
        {
            // Overload method giống như GetNewsDetail
            return AppProcessor.ProcedureProvider.ExecuteTypedList<ReadingScheduleModel>(
                _API_KhachHang_GetReadingSchedule,
                DATA_PROVIDER_NAME,
                contractId
            );
        }
    }
}