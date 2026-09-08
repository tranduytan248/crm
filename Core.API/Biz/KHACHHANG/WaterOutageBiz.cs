using Core.API.Models;
using Core.Cate.Biz;
using Modules.API.Models.KhachHang;
using Modules.API.Models.WaterOutageModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TSFramework.Libs.Processors;

namespace Core.API.Biz.KHACHHANG
{
    public class WaterOutageBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private const string _API_WaterOutages_ByContractId = "API_WaterOutages_ByContractId";
        private const string _API_KhachHang_GetWaterOutagesByArea = "API_KhachHang_GetWaterOutagesByArea";
        private const string _API_KhachHang_GetWaterOutages = "KWC_WaterOutages_GetList";


        public static List<WaterOutageModel> GetOutagesByContract(int contractId)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<WaterOutageModel>(_API_WaterOutages_ByContractId
               , DATA_PROVIDER_NAME, contractId);
        }
        public static List<WaterOutageModel> GetOutagesByArea(string area)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<WaterOutageModel>(
                _API_KhachHang_GetWaterOutagesByArea, DATA_PROVIDER_NAME, area);
        }

        public static List<WaterOutageModel> GetList()
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<WaterOutageModel>(
                _API_KhachHang_GetWaterOutages, DATA_PROVIDER_NAME);
        }
    }
}

