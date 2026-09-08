using Core.Sys.Models.Sys;
using TSFramework.Libs.Processors;

namespace Core.Sys.Biz.Sys
{
    public class SysPermissionAPIBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";
        private readonly string _sysPermissionAPISave = "Sys_PhanQuyen_API_Save";
        private readonly string _sysPhanQuyenAPIGetByUserID = "Sys_PhanQuyen_API_GetByUserID";

        public SysPermissionAPIModel GetById(int userID)
        {
            var configs = LoadDetail(userID);
            return configs;
        }

        private SysPermissionAPIModel LoadDetail(int configId)
        {
            var dataPermissionAPI =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysPermissionAPIModel>(_sysPhanQuyenAPIGetByUserID,
                    DATA_PROVIDER_NAME, configId);
            return dataPermissionAPI;
        }

        public int Save(SysPermissionAPIModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysPermissionAPISave, DATA_PROVIDER_NAME,
                model.UserID,
                model.ApplyFor
             );

            return result.GetValueOrDefault(0);
        }       
    }
}