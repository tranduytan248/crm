using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_DigitalSalesBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";

        private readonly string _spGetList = "RM_DigitalSales_GetList";
        private readonly string _spGetByID = "RM_DigitalSales_GetByID";
        private readonly string _spSave = "RM_DigitalSales_Save";
        private readonly string _spProductGetBySalesID = "RM_DigitalSalesProduct_GetBySalesID";
        private readonly string _spProductSave = "RM_DigitalSalesProduct_Save";
        private readonly string _spProductDelete = "RM_DigitalSalesProduct_Delete";
        private readonly string _spChangeStatus = "RM_DigitalSales_ChangeStatus";
        private readonly string _spTrackingGetBySalesID = "RM_DigitalSalesTracking_GetBySalesID";
        private readonly string _spTrackingSave = "RM_DigitalSalesTracking_Save";
        private readonly string _spTrackingDelete = "RM_DigitalSalesTracking_Delete";
        private readonly string _spTrackingUpdateStatus = "RM_DigitalSalesTracking_UpdateStatus";
        private readonly string _spMemberGetBySalesID = "RM_DigitalSalesMember_GetBySalesID";
        private readonly string _spMemberSave = "RM_DigitalSalesMember_Save";
        private readonly string _spMemberDelete = "RM_DigitalSalesMember_Delete";
        private readonly string _spGetTimeline = "RM_DigitalSales_GetTimeline";
        private readonly string _spDelete = "RM_DigitalSales_Delete";
        private readonly string _spStatusGetAll = "RM_DigitalSalesStatus_GetAll";

        public List<RM_DigitalSalesModel> LoadList(out int total, RM_DigitalSalesSearchModel model)
        {
            total = 0;
            DateTime? fromDate = null;
            DateTime? toDate = null;

            if (!string.IsNullOrWhiteSpace(model.FromDate))
            {
                if (DateTime.TryParseExact(model.FromDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fd))
                    fromDate = fd;
            }

            if (!string.IsNullOrWhiteSpace(model.ToDate))
            {
                if (DateTime.TryParseExact(model.ToDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime td))
                    toDate = td;
            }

            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_DigitalSalesModel>(
                _spGetList,
                DATA_PROVIDER_NAME,
                string.IsNullOrWhiteSpace(model.Keyword) ? null : model.Keyword.Trim(),
                model.BusinessType,
                model.StatusID,
                model.CustomerID,
                model.ProductServiceID,
                model.DepartmentID,
                model.EmployeeID,
                fromDate,
                toDate,
                model.PageNumber <= 0 ? 1 : model.PageNumber,
                model.PageSize <= 0 ? 20 : model.PageSize,
                model.UserName
            );

            if (data != null && data.Count > 0)
            {
                total = data.First().TotalCount.GetValueOrDefault(0);
            }

            return data ?? new List<RM_DigitalSalesModel>();
        }

        public RM_DigitalSalesModel GetByID(int id)
        {
            if (id <= 0) return null;
            var model = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_DigitalSalesModel>(_spGetByID, DATA_PROVIDER_NAME, id);
            if (model != null)
            {
                model.Products = GetProductsBySalesID(id);
                model.Members = GetMembersBySalesID(id);
                model.TrackingTasks = GetTrackingTasks(id);
                model.Timelines = GetTimeline(id);
            }
            return model;
        }

        public int Save(RM_DigitalSalesModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _spSave,
                DATA_PROVIDER_NAME,
                model.DigitalSalesID,
                model.Code,
                model.Title,
                model.BusinessType,
                model.StatusID,
                model.CustomerID,
                model.ContactPerson_ID,
                model.ClosingProbability,
                model.ExpectedDate,
                model.StartDate,
                model.EndDate,
                model.ContractID,
                model.ContractNo,
                model.ContractValue,
                model.ContractSignDate,
                model.AssignedEmployeeID,
                model.DepartmentID,
                model.Note,
                model.FileAttach,
                username
            );

            return result.GetValueOrDefault(0);
        }

        public List<RM_DigitalSalesProductModel> GetProductsBySalesID(int digitalSalesId)
        {
            if (digitalSalesId <= 0) return new List<RM_DigitalSalesProductModel>();
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_DigitalSalesProductModel>(
                _spProductGetBySalesID,
                DATA_PROVIDER_NAME,
                digitalSalesId
            );
            return data ?? new List<RM_DigitalSalesProductModel>();
        }

        public int SaveProduct(RM_DigitalSalesProductModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _spProductSave,
                DATA_PROVIDER_NAME,
                model.SalesProductID,
                model.DigitalSalesID,
                model.ProductServiceID,
                model.ExpectedRevenue,
                model.ActualRevenue,
                model.PackageName,
                model.Quantity <= 0 ? 1 : model.Quantity,
                model.StartDate,
                model.EndDate,
                model.Note,
                username
            );
            return result.GetValueOrDefault(0);
        }

        public int DeleteProduct(int salesProductId, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _spProductDelete,
                DATA_PROVIDER_NAME,
                salesProductId,
                username
            );
            return result.GetValueOrDefault(0);
        }

        public int ChangeStatus(int digitalSalesId, int newStatusId, string note, string attachmentPath, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _spChangeStatus,
                DATA_PROVIDER_NAME,
                digitalSalesId,
                newStatusId,
                note,
                attachmentPath,
                username
            );

            return result.GetValueOrDefault(0);
        }

        public List<RM_DigitalSalesTrackingModel> GetTrackingTasks(int digitalSalesId)
        {
            if (digitalSalesId <= 0) return new List<RM_DigitalSalesTrackingModel>();
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_DigitalSalesTrackingModel>(
                _spTrackingGetBySalesID,
                DATA_PROVIDER_NAME,
                digitalSalesId
            );
            return list ?? new List<RM_DigitalSalesTrackingModel>();
        }

        public int UpdateTrackingStatus(int trackingId, byte status, string resultNote, string attachmentFile, int? assignedUserId, DateTime? deadline, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _spTrackingUpdateStatus,
                DATA_PROVIDER_NAME,
                trackingId,
                status,
                resultNote,
                attachmentFile,
                assignedUserId.HasValue ? (object)assignedUserId.Value : DBNull.Value,
                deadline.HasValue ? (object)deadline.Value : DBNull.Value,
                username
            );
            return result.GetValueOrDefault(0);
        }

        public int SaveTracking(RM_DigitalSalesTrackingModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _spTrackingSave,
                DATA_PROVIDER_NAME,
                model.TrackingID,
                model.DigitalSalesID,
                model.ProcessID.HasValue ? (object)model.ProcessID.Value : DBNull.Value,
                model.ProgressID.HasValue ? (object)model.ProgressID.Value : DBNull.Value,
                model.TaskName,
                model.AssignedUserID.HasValue ? (object)model.AssignedUserID.Value : DBNull.Value,
                model.StartDate,
                model.Deadline.HasValue ? (object)model.Deadline.Value : DBNull.Value,
                model.Status,
                model.ResultNote,
                model.AttachmentFile,
                model.IsCustomTask,
                model.SortOrder,
                username
            );
            return result.GetValueOrDefault(0);
        }

        public int DeleteTracking(int trackingId, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _spTrackingDelete,
                DATA_PROVIDER_NAME,
                trackingId,
                username
            );
            return result.GetValueOrDefault(0);
        }

        public List<RM_DigitalSalesMemberModel> GetMembersBySalesID(int digitalSalesId)
        {
            if (digitalSalesId <= 0) return new List<RM_DigitalSalesMemberModel>();
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_DigitalSalesMemberModel>(
                _spMemberGetBySalesID,
                DATA_PROVIDER_NAME,
                digitalSalesId
            );
            return list ?? new List<RM_DigitalSalesMemberModel>();
        }

        public int SaveMember(RM_DigitalSalesMemberModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _spMemberSave,
                DATA_PROVIDER_NAME,
                model.MemberID,
                model.DigitalSalesID,
                model.UserID,
                model.RoleTitle,
                model.IsAM,
                model.Note,
                username
            );
            return result.GetValueOrDefault(0);
        }

        public int DeleteMember(int memberId, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _spMemberDelete,
                DATA_PROVIDER_NAME,
                memberId,
                username
            );
            return result.GetValueOrDefault(0);
        }

        public List<RM_DigitalSalesTimelineModel> GetTimeline(int digitalSalesId)
        {
            if (digitalSalesId <= 0) return new List<RM_DigitalSalesTimelineModel>();
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_DigitalSalesTimelineModel>(
                _spGetTimeline,
                DATA_PROVIDER_NAME,
                digitalSalesId
            );
            return list ?? new List<RM_DigitalSalesTimelineModel>();
        }

        public int Delete(int digitalSalesId, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _spDelete,
                DATA_PROVIDER_NAME,
                digitalSalesId,
                username
            );
            return result.GetValueOrDefault(0);
        }

        public List<RM_DigitalSalesStatusModel> GetStatusList(byte? businessType = null)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_DigitalSalesStatusModel>(
                _spStatusGetAll,
                DATA_PROVIDER_NAME,
                businessType.HasValue ? (object)businessType.Value : DBNull.Value
            );
            return list ?? new List<RM_DigitalSalesStatusModel>();
        }

        public string GenerateNextCode()
        {
            try
            {
                var currentYear = DateTime.Now.Year.ToString();
                int total = 0;
                var latest = LoadList(out total, new RM_DigitalSalesSearchModel());
                int maxId = 0;
                if (latest != null && latest.Count > 0)
                {
                    maxId = latest.Max(x => x.DigitalSalesID);
                }
                int nextSeq = maxId + 1;
                return $"SPDV-{currentYear}-{nextSeq:D4}";
            }
            catch
            {
                return $"SPDV-{DateTime.Now.Year}-0001";
            }
        }
    }
}
