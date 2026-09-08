using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Core.Cate.Models;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_CustomerAnniversaryBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";

        private readonly string _RM_CustomerAnniversary_Save = "RM_CustomerAnniversary_Save";
        private readonly string _RM_CustomerAnniversary_Get = "RM_CustomerAnniversary_Get";
        private readonly string _RM_CustomerAnniversary_Delete = "RM_CustomerAnniversary_Delete";
        private readonly string _RM_CustomerAnniversary_GetByID = "RM_CustomerAnniversary_GetByID";
        private readonly string _RM_AnniversaryType_Get = "RM_AnniversaryType_Get";

        public List<RM_CustomerAnniversaryModel> LoadList(out int total,
    RM_CustomerAnniversarySearchModel searchModel,
    BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            if (search.PageSize <= 0) search.PageSize = int.MaxValue;

            searchModel = searchModel ?? new RM_CustomerAnniversarySearchModel();

            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_CustomerAnniversaryModel>(
                _RM_CustomerAnniversary_Get,
                DATA_PROVIDER_NAME,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize,  
                searchModel.CustomerID);

            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        public RM_CustomerAnniversaryModel LoadDetail(int id)
        {
            return AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_CustomerAnniversaryModel>(
                _RM_CustomerAnniversary_GetByID, DATA_PROVIDER_NAME, id);
        }

        public int Save(RM_CustomerAnniversaryModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _RM_CustomerAnniversary_Save,
                DATA_PROVIDER_NAME,
                model.CustomerAnniversaryId,
                model.CustomerID,
                model.AnniversaryTypeId,
                model.AnniversaryDate,
                model.Note,
                model.IsLunar,
                username);
            return result.GetValueOrDefault(0);
        }

        public int Delete(RM_CustomerAnniversaryModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(
                _RM_CustomerAnniversary_Delete,
                DATA_PROVIDER_NAME,
                model.CustomerAnniversaryId,
                username);
            return result.GetValueOrDefault(0);
        }

        public List<SelectListItem> GetAllAnniversaryType()
        {
            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_AnniversaryTypeModel>(
                _RM_AnniversaryType_Get, DATA_PROVIDER_NAME,
                null,   // @Search
                "0",    // @Order
                "ASC",  // @OrderDir
                0,      // @StartIndex
                -1);    // @PageSize

            if (data == null) return new List<SelectListItem>();

            return data.Select(d => new SelectListItem
            {
                Text = d.NameAnniversaryType,
                Value = d.AnniversaryType_ID.ToString()
            }).ToList();
        }
    }
}