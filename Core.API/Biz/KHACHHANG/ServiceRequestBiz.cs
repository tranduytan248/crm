using Core.API.Models;
using Core.Cate.Biz;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Modules.API.Models.KhachHang;
using Modules.API.Models.NewsKhachHang;
using Modules.API.Models.ServiceRequestModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TSFramework.Libs.Processors;

namespace Core.API.Biz.KHACHHANG
{
    public class ServiceRequest
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _API_ServiceRequests_RelocateMeter = "API_ServiceRequests_RelocateMeter";

        public ServiceRequestModel CreateRelocateMeter(ServiceRequestModel request)
        {
            // Gọi stored procedure trả về danh sách
            var resultList = AppProcessor.ProcedureProvider.ExecuteTypedList<ServiceRequestModel>(
                _API_ServiceRequests_RelocateMeter,
                DATA_PROVIDER_NAME,
                request.ContractId,
                request.RequesterName,
                request.Phone,
                request.Email,
                request.RequestType,
                request.RequestDetails
            );


            // Trả về phần tử đầu tiên hoặc null nếu không có
            return resultList?.FirstOrDefault();
        }
    }
}
