using Core.Sys.Models.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Core.Sys.Biz.Sys
{
    public class SysUserIPLockBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Sys";

        private readonly string _sysUser_IPLock_GetByIP = "Sys_User_IPLock_GetByIP";
        private readonly string _sysUser_IPLock_Save = "Sys_User_IPLock_Save";


        /// <summary>
        /// Kiểm tra
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public SysUserIPLockModel GetByIP(string IP)
        {
            var data =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<SysUserIPLockModel>(_sysUser_IPLock_GetByIP,
                    DATA_PROVIDER_NAME, IP);
            return data;
        }


        /// <summary>
        /// Lock/Unlock IP
        /// </summary>
        /// <param name="model"></param>
        /// <param name="lockIp"></param>
        /// <returns></returns>
        public int Save(string IP, bool lockIp)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_sysUser_IPLock_Save, DATA_PROVIDER_NAME,
                IP,
                lockIp
            );
            return result.GetValueOrDefault(0);
        }
    }
}
