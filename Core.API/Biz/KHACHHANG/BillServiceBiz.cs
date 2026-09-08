using Core.API.Models;
using Core.Cate.Biz;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Modules.API.Models.KhachHang;
using Modules.API.Models.NewsKhachHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TSFramework.Libs.Processors;

namespace Core.API.Biz.KHACHHANG
{
    public class BillServiceBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _API_KhachHang_GetBillByMonth = "API_KhachHang_GetBillByMonth";

        public BillResponseModel GetBillByMonth(int contractId, int month, int year)
        {
            // Giữ nguyên kiểu int, không ép sang string
            var resultList = AppProcessor.ProcedureProvider.ExecuteTypedList<BillResponseModel>(
                _API_KhachHang_GetBillByMonth,
                DATA_PROVIDER_NAME,
                contractId,
                month,
                year
            );

            return resultList?.FirstOrDefault();
        }
    }
}