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
    public class CustomerContractsBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _API_KhachHang_GetContracts = "API_KhachHang_GetContracts";
        public List<CustomerContractsModel> GetContracts(int userId)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<CustomerContractsModel>(
                _API_KhachHang_GetContracts,DATA_PROVIDER_NAME,userId);
        }
    }
}
