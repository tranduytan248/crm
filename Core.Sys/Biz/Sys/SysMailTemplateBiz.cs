using System.Collections.Generic;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Sys.Biz.Sys
{
    /// <summary>
    /// Biz thao tác dữ liệu mẫu email trong hệ thống.
    /// </summary>
    public class SysMailTemplateBiz
    {
        #region Declaration

        private const string DataProviderName = "CenIT.Provider.Sys";
        private const string MailTemplateGetProcedure = "Sys_MailTemplate_Get";
        private const string MailTemplateGetByIdProcedure = "Sys_MailTemplate_GetByID";
        private const string MailTemplateGetActiveByCodeProcedure = "Sys_MailTemplate_GetActiveByCode";
        private const string MailTemplateSaveProcedure = "Sys_MailTemplate_Save";
        private const string MailTemplateDeleteProcedure = "Sys_MailTemplate_Delete";
        private const string MailTemplateParamGetByTemplateIdProcedure = "Sys_MailTemplateParam_GetByTemplateId";
        private const string MailTemplateParamGetByTemplateCodeProcedure = "Sys_MailTemplateParam_GetByTemplateCode";
        private const string MailTemplateParamSaveProcedure = "Sys_MailTemplateParam_Save";
        private const string MailTemplateParamDeleteByTemplateIdProcedure = "Sys_MailTemplateParam_DeleteByTemplateId";

        #endregion

        #region Public Methods

        /// <summary>
        /// Lấy danh sách mẫu email theo điều kiện tìm kiếm.
        /// </summary>
        public List<SysMailTemplateModel> Get(out int total, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel();

            List<SysMailTemplateModel> templates = AppProcessor.ProcedureProvider.ExecuteTypedList<SysMailTemplateModel>(
                MailTemplateGetProcedure,
                DataProviderName,
                search.Search);

            total = templates?.Count ?? 0;
            return templates ?? new List<SysMailTemplateModel>();
        }

        /// <summary>
        /// Lấy chi tiết mẫu email theo mã dữ liệu.
        /// </summary>
        public SysMailTemplateModel GetById(int mailTemplateId)
        {
            SysMailTemplateModel template = AppProcessor.ProcedureProvider.ExecuteScalarObject<SysMailTemplateModel>(
                MailTemplateGetByIdProcedure,
                DataProviderName,
                mailTemplateId);

            return template;
        }

        /// <summary>
        /// Lấy mẫu email đang hoạt động theo mã template.
        /// </summary>
        public SysMailTemplateModel GetActiveByCode(string templateCode)
        {
            SysMailTemplateModel template = AppProcessor.ProcedureProvider.ExecuteScalarObject<SysMailTemplateModel>(
                MailTemplateGetActiveByCodeProcedure,
                DataProviderName,
                templateCode);

            return template;
        }

        /// <summary>
        /// Lưu thông tin mẫu email.
        /// </summary>
        public int Save(SysMailTemplateModel model, string updatedBy)
        {
            int? mailTemplateId = AppProcessor.ProcedureProvider.Execute(
                MailTemplateSaveProcedure,
                DataProviderName,
                model.MailTemplateId,
                model.TemplateCode,
                model.TemplateName,
                model.SubjectTemplate,
                model.FilePath,
                model.IsActive,
                model.Description,
                updatedBy);

            return mailTemplateId.GetValueOrDefault(0);
        }

        /// <summary>
        /// Xóa mẫu email theo mã dữ liệu.
        /// </summary>
        public int Delete(int mailTemplateId, string updatedBy)
        {
            int? result = AppProcessor.ProcedureProvider.Execute(
                MailTemplateDeleteProcedure,
                DataProviderName,
                mailTemplateId,
                updatedBy);

            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Lấy danh sách tham số theo mã mẫu email.
        /// </summary>
        public List<SysMailTemplateParamModel> GetParamsByTemplateId(int mailTemplateId)
        {
            List<SysMailTemplateParamModel> parameters =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysMailTemplateParamModel>(
                    MailTemplateParamGetByTemplateIdProcedure,
                    DataProviderName,
                    mailTemplateId);

            return parameters ?? new List<SysMailTemplateParamModel>();
        }

        /// <summary>
        /// Lấy danh sách tham số theo mã template.
        /// </summary>
        public List<SysMailTemplateParamModel> GetParamsByTemplateCode(string templateCode)
        {
            List<SysMailTemplateParamModel> parameters =
                AppProcessor.ProcedureProvider.ExecuteTypedList<SysMailTemplateParamModel>(
                    MailTemplateParamGetByTemplateCodeProcedure,
                    DataProviderName,
                    templateCode);

            return parameters ?? new List<SysMailTemplateParamModel>();
        }

        /// <summary>
        /// Lưu thông tin tham số của mẫu email.
        /// </summary>
        public int SaveParam(SysMailTemplateParamModel model, string updatedBy)
        {
            int? mailTemplateParamId = AppProcessor.ProcedureProvider.Execute(
                MailTemplateParamSaveProcedure,
                DataProviderName,
                model.MailTemplateParamId,
                model.MailTemplateId,
                model.ParamCode,
                model.ParamName,
                model.IsRequired,
                model.DefaultValue,
                updatedBy);

            return mailTemplateParamId.GetValueOrDefault(0);
        }

        /// <summary>
        /// Xóa toàn bộ tham số của một mẫu email.
        /// </summary>
        public int DeleteParamsByTemplateId(int mailTemplateId, string updatedBy)
        {
            int? result = AppProcessor.ProcedureProvider.Execute(
                MailTemplateParamDeleteByTemplateIdProcedure,
                DataProviderName,
                mailTemplateId,
                updatedBy);

            return result.GetValueOrDefault(0);
        }

        #endregion
    }
}
