using Core.API.Models;
using Core.Cate.Biz;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Modules.API.Models.KhachHang;
using Modules.API.Models.NewsKhachHang;
using Modules.API.Models.WaterOutageModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TSFramework.Libs.Processors;

namespace Core.API.Biz.KHACHHANG
{
    public class WaterComplaintBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string API_KhachHang_CreateWaterComplaint = "API_KhachHang_CreateWaterComplaint";

        /// <summary>
        /// Tạo mới khiếu nại về nước
        /// </summary>
        /// <param name="fullName">Họ tên khách hàng</param>
        /// <param name="contractCode">Mã hợp đồng (string)</param>
        /// <param name="phone">Số điện thoại</param>
        /// <param name="location">Địa chỉ</param>
        /// <param name="reportContent">Nội dung báo cáo</param>
        /// <param name="attachmentFileName">Tên file đính kèm</param>
        /// <returns>Kết quả tạo khiếu nại</returns>
        public WaterComplaintResultModel CreateComplaint(
            string fullName,
            string contractCode,
            string phone,
            string location,
            string reportContent,
            string attachmentFileName
        )
        {
            try
            {
                var result = AppProcessor.ProcedureProvider
                    .ExecuteTypedList<WaterComplaintResultModel>(
                        API_KhachHang_CreateWaterComplaint,
                        DATA_PROVIDER_NAME,
                        fullName,
                        contractCode,
                        phone,
                        location,
                        reportContent,
                        attachmentFileName
                    ).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể tạo khiếu nại: {ex.Message}", ex);
            }
        }
    }
}