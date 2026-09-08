using Core.API.Models;
using Core.Cate.Biz;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Modules.API.Models.KhachHang;
using Modules.API.Models.NewsKhachHang;
using Modules.API.Models.UsageAlertModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TSFramework.Libs.Processors;
using UsageAlertModel = Modules.API.Models.UsageAlertModel.UsageAlertModel;

namespace Core.API.Biz.KHACHHANG
{
    public class ContractBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _API_KhachHang_GetUsageAlert = "API_KhachHang_GetUsageAlert";
        public UsageAlertModel GetUsageAlert(int contractId)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<UsageAlertModel>(
                _API_KhachHang_GetUsageAlert,
                DATA_PROVIDER_NAME,
                contractId);

            return list?.FirstOrDefault();
        }
    }
}
