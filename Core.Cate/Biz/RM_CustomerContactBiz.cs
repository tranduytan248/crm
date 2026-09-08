using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Helpers;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_CustomerContactBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _RM_CustomerContact_Save = "RM_CustomerContact_Save";
        private readonly string _RM_CustomerContact_Get = "RM_CustomerContact_Get";
        private readonly string _RM_CustomerContact_Delete = "RM_CustomerContact_Delete";
        private readonly string _RM_CustomerContact_GetByID = "RM_CustomerContact_GetByID";

        #region ===== GET LIST =====
        public List<RM_CustomerContactModel> LoadList(out int total, int? customerID, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_CustomerContactModel>(
                _RM_CustomerContact_Get,
                DATA_PROVIDER_NAME,
                customerID,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize
            );

            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }
        #endregion

        #region ===== GET DETAIL =====
        public RM_CustomerContactModel LoadDetail(int id)
        {
            return AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_CustomerContactModel>(
                _RM_CustomerContact_GetByID,
                DATA_PROVIDER_NAME,
                id
            );
        }
        #endregion

        #region ===== GET ALL =====
        public List<RM_CustomerContactModel> GetAll()
        {
            int total;
            return LoadList(out total, null, null);
        }
        #endregion

        #region ===== SAVE =====
        public int Save(RM_CustomerContactModel model, string username)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var result = AppProcessor.ProcedureProvider.Execute(
                _RM_CustomerContact_Save,
                DATA_PROVIDER_NAME,
                model.CustomerContactID,
                model.CustomerID,
                model.ContactPersonID,
                model.Position,
                model.Note,
                model.Email,    
                username
            );

            return result.GetValueOrDefault(0);
        }
        #endregion

        #region ===== DELETE =====
        public int Delete(int id, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _RM_CustomerContact_Delete,
                DATA_PROVIDER_NAME,
                id,
                username
            );

            return result.GetValueOrDefault(0);
        }
        #endregion
    }
}
