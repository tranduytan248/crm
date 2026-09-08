using Core.Log.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Log.Biz
{
    public class AccessHistoryBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";
        private readonly string _dl_AccessHistories_Save = "AccessHistories_Save";
        private readonly string _dl_AccessHistories_Get = "AccessHistories_Get";

        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns></returns>
        private List<AccessHistoryModel> LoadList(out int total, AccessHistorySearchModel searchModel, BaseSearchModel search)
        {
            search = search ?? new BaseSearchModel
            {
                Search = null,
                Order = "1",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            var dataXH = AppProcessor.ProcedureProvider.ExecuteTypedList<AccessHistoryModel>(_dl_AccessHistories_Get,
                DATA_PROVIDER_NAME,
                searchModel.TuNgay,
                searchModel.DenNgay,
                searchModel.TuKhoa,
                search.Search,
                search.Order,
                search.OrderDir,
                search.StartIndex,
                search.PageSize);

            total = 0;
            if (dataXH != null && dataXH.Count > 0)
                total = int.Parse(dataXH.First()?.TotalRow.ToString() ?? "0");
            return dataXH;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách liên kết theo giá trị lọc
        /// </summary>
        /// <returns></returns>
        public List<AccessHistoryModel> GetList(out int total, AccessHistorySearchModel searchModel, BaseSearchModel search = null)
        {
            var dataXH = LoadList(out total, searchModel, search);
            return dataXH;
        }


        /// <summary>
        /// luu thong tin vao database
        /// </summary>
        /// <returns></returns>
        public int Save(AccessHistoryModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_dl_AccessHistories_Save, DATA_PROVIDER_NAME,
                model.Area,
                model.Controller,
                model.Action,
                model.CreatedBy
                );

            return result.GetValueOrDefault(0);
        }
    }
}