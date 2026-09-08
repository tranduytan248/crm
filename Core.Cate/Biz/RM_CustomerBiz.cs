using Core.Cate.Models;
using Core.RM.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Security.Policy;
using System.Web.Helpers;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class RM_CustomerBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private const string CONN_NAME = "TOC.Conn.Major";
        private readonly string _RM_Customer_Save = "RM_Customer_Save";
        private readonly string _RM_Customer_Get = "RM_Customer_Get";
        private readonly string _RM_Customer_Delete = "RM_Customer_Delete";
        private readonly string _RM_Customer_GetByID = "RM_Customer_GetByID";
        private readonly string _RM_BusinessOpportunity_Save = "RM_BusinessOpportunity_Save";
        private readonly string _RM_BusinessOpportunity_GetByCustomerId = "RM_BusinessOpportunity_GetByCustomerId";
        private readonly string _RM_Customer_BulkImport = "RM_Customer_BulkImport";



        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách RM_Customer</returns>
        public List<RM_CustomerModel> LoadList(out int total, RM_CustomerSearchModel searchModel, BaseSearchModel search)
        {
            search = search ?? new RM_CustomerSearchModel
            {
                Order = "0",
                OrderDir = "ASC",
                StartIndex = 0,
                PageSize = -1
            };

            searchModel = searchModel ?? new RM_CustomerSearchModel();

            var data = AppProcessor.ProcedureProvider.ExecuteTypedList<RM_CustomerModel>(
                 _RM_Customer_Get,
                 DATA_PROVIDER_NAME,
                 search.Search,
                 search.Order,
                 search.OrderDir,
                 search.StartIndex,
                 search.PageSize,
                 searchModel.Keyword,
                 searchModel.CustomerStatusID,
                 searchModel.CustomerTypeID);

            total = 0;
            if (data != null && data.Count > 0)
                total = int.Parse(data.First()?.TotalRow.ToString() ?? "0");
            return data;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách RM_Customer
        /// </summary>
        /// <returns>Danh sách RM_Customer</returns>
        public List<RM_CustomerModel> GetAll()
        {
            int total;
            var list = LoadList(out total, null, null);
            return list;
        }

        /// <summary>
        /// Lấy danh sách RM_Customer theo ID
        /// </summary>
        /// <returns>Danh sách RM_Customer</returns>
        public RM_CustomerModel LoadDetail(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_CustomerModel>(_RM_Customer_GetByID, DATA_PROVIDER_NAME, ID);
            return data;
        }

        /// <summary>
        /// Xóa danh sách RM_Customer theo ID
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Delete(RM_CustomerModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_Customer_Delete, DATA_PROVIDER_NAME, model.CustomerID, username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Cập nhật danh sách RM_Customer theo dữ liệu đầu vào
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int Save(RM_CustomerModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_Customer_Save, DATA_PROVIDER_NAME
                , model.CustomerID
                , model.CustomerTypeID
                , model.CustomerName
                , model.ShortName
                , model.TaxCode
                , model.CompanyType
                , model.EstablishmentDate
                , model.CustomerStatusID
                , model.Website
                , model.CharterCapital
                , model.AddressCus
                , model.Email
                , model.Phone
                , model.Fax
               , username);
            int customerId = result.GetValueOrDefault(0);

            return customerId;
        }

        /// <summary>
        /// Thêm mới Cơ hội kinh doanh
        /// </summary>
        /// <returns> Kết quả thực hiện</returns>
        public int SaveCHKD(RM_BusinessOpportunityModel model, string username)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_RM_BusinessOpportunity_Save, DATA_PROVIDER_NAME,
                model.BusinessOpportunityID,
                model.OpportunityName,
                model.CustomerID,
                model.ContactPerson_ID,
                model.ExchangeDate,
                model.SalesStageID,
                model.ClosingProbability,
                model.ExpectedValue,
                model.StatusID,
                model.Description,
                model.FileAttach,
                model.ProductServiceIDs,
                username);
            return result.GetValueOrDefault(0);
        }

        /// <summary>
        /// Lấy danh sách Cơ hội kinh doanh theo Customer ID
        /// </summary>
        /// <returns>danh sách Cơ hội kinh doanh</returns>
        public RM_BusinessOpportunityModel LoadData_ByCustormerId(int ID)
        {
            var data = AppProcessor.ProcedureProvider.ExecuteScalarObject<RM_BusinessOpportunityModel>(_RM_BusinessOpportunity_GetByCustomerId, DATA_PROVIDER_NAME, ID);
            return data;
        }
        public RM_CustomerImportResultModel BulkImport(List<RM_CustomerImportRowModel> rows, string username)
        {
            var dt = BuildCustomerDataTable(rows);
            var duplicates = new List<RM_CustomerDuplicateModel>();

            var connectionString = System.Web.Configuration.WebConfigurationManager
                .ConnectionStrings[CONN_NAME]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
                throw new Exception($"Không tìm thấy ConnectionString '{CONN_NAME}'");

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(_RM_Customer_BulkImport, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;

                var tvpParam = cmd.Parameters.AddWithValue("@customers", dt);
                tvpParam.SqlDbType = SqlDbType.Structured;
                tvpParam.TypeName = "dbo.CustomerTableType";

                cmd.Parameters.AddWithValue("@created_by", username);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        duplicates.Add(new RM_CustomerDuplicateModel
                        {
                            CustomerName = reader["CustomerName"]?.ToString(),
                            TaxCode = reader["TaxCode"]?.ToString(),
                            Reason = reader["Reason"]?.ToString()
                        });
                    }
                }
            }

            return new RM_CustomerImportResultModel
            {
                SuccessCount = rows.Count - duplicates.Count,
                FailCount = duplicates.Count,
                DuplicateRows = duplicates
            };
        }
        private DataTable BuildCustomerDataTable(List<RM_CustomerImportRowModel> rows)
        {
            var dt = new DataTable();
            dt.Columns.Add("CustomerTypeID", typeof(byte));
            dt.Columns.Add("CustomerName", typeof(string));
            dt.Columns.Add("ShortName", typeof(string));
            dt.Columns.Add("TaxCode", typeof(string));
            dt.Columns.Add("CompanyType", typeof(string));
            dt.Columns.Add("EstablishmentDate", typeof(DateTime));
            dt.Columns.Add("CustomerStatusID", typeof(byte));
            dt.Columns.Add("Website", typeof(string));
            dt.Columns.Add("CharterCapital", typeof(decimal));
            dt.Columns.Add("AddressCus", typeof(string));
            dt.Columns.Add("Province", typeof(string));
            dt.Columns.Add("Ward", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("Phone", typeof(string));
            dt.Columns.Add("Fax", typeof(string));

            foreach (var r in rows)
            {
                dt.Rows.Add(
                    r.CustomerTypeID,
                    r.CustomerName,
                    r.ShortName,
                    r.TaxCode,
                    r.CompanyType,
                    r.EstablishmentDate.HasValue ? (object)r.EstablishmentDate.Value : DBNull.Value,
                    r.CustomerStatusID,
                    r.Website,
                    r.CharterCapital == 0 ? (object)DBNull.Value : r.CharterCapital,
                    r.AddressCus,
                    r.Province,
                    r.Ward,
                    r.Email,
                    r.Phone,
                    r.Fax
                );
            }
            return dt;
        }
    }
}