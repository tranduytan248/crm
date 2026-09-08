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
    public class NewsBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _API_KhachHang_GetNewsList = "API_KhachHang_GetNewsList";
        private readonly string _API_KhachHang_GetNewsCategories = "API_KhachHang_GetNewsCategories";
        private readonly string _API_KhachHang_GetNewsDetail = "API_KhachHang_GetNewsDetail";

        public List<UsageAlertModel> GetNewsList()
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<UsageAlertModel>(_API_KhachHang_GetNewsList,DATA_PROVIDER_NAME);
        }
        public List<NewsCategoryModel> GetNewsCategory()
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<NewsCategoryModel>
                (_API_KhachHang_GetNewsCategories, DATA_PROVIDER_NAME);
        }
        public NewsDetailModel GetNewsDetail(int id)
        {
            var list = AppProcessor.ProcedureProvider.ExecuteTypedList<NewsDetailModel>(_API_KhachHang_GetNewsDetail, DATA_PROVIDER_NAME, id);
            return list?.FirstOrDefault();
        }
    }
}
