using Core.Sys.Models.Sys;
using TSFramework.Libs.Processors;

namespace Core.Sys.Biz.Sys
{
    public class SysIpRequestBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";
        private readonly string _sysIpRequestGetByIp = "Sys_IPRequest_GetByIp";

        public SysIPRequestModel GetByIP(string sIp)
        {
            var modelIP =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysIPRequestModel>(_sysIpRequestGetByIp,
                    DATA_PROVIDER_NAME, sIp);
            return modelIP;
        }
    }
}