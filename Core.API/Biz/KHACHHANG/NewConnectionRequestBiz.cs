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
    public class NewConnectionRequestBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _API_ServiceRequests_NewConnection = "API_ServiceRequests_NewConnection";

        public NewConnectionRequestModel CreateNewConnection(NewConnectionRequestModel request)
        {
            // Gọi stored procedure trả về danh sách
            var resultList = AppProcessor.ProcedureProvider.ExecuteTypedList<NewConnectionRequestModel>(
                _API_ServiceRequests_NewConnection,
                DATA_PROVIDER_NAME,
                request.ServiceType,
                request.FullNameOrOrganization,
                request.DateOfBirth,
                request.Identity.Type,
                request.Identity.Number,
                request.Identity.IssueDate,
                request.Identity.IssuedPlace,
                request.Contact.PhoneNumber,
                request.Contact.Email ?? (object)DBNull.Value,
                request.RegistrationAddress,
                request.UsagePurpose
            );


            // Trả về phần tử đầu tiên hoặc null nếu không có
            return resultList?.FirstOrDefault();
        }
    }
}
