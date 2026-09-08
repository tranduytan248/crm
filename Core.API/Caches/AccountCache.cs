using Core.API.Biz;
using Core.API.Models;
//using Core.Cate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Caching;
using KWC_AccountMobileModel = Core.API.Models.KWC_AccountMobileModel;

namespace Core.API.Caches
{
    public class AccountCache : CacheLayer
    {
        private AccountBiz _KWC_AccountMobileApi;
        protected override string[] MasterCacheKeyArray => new[] { "KWC_AccountMobileCache", "CENIT.APP.Cache" };
        private AccountBiz Api => _KWC_AccountMobileApi ?? (_KWC_AccountMobileApi = new AccountBiz());

        // Kiểm tra tk tồn tại
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public int IsExist(KWC_AccountMobileModel model)
        {
            return Api.IsExist(model);
        }

        // Đăng nhập
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Login(KWC_AccountMobile_LoginModel model)
        {
            var login = Api.Login(model);
            if (login > 0) InvalidateCache();
            return login;
        }

        // Tạo tài khoản 
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public int Create(KWC_AccountMobileModel model)
        {
            var login = Api.Create(model);
            if (login > 0) InvalidateCache();
            return login;
        }

        //// Kiểm tra tk tồn tại
        //[DataObjectMethod(DataObjectMethodType.Select, false)]
        //public CSKH_KH_CONTRACT_Model GetInfoContract(string contract, string phone)
        //{
        //    return Api.GetInfoContract(contract, phone);
        //}
    }
}
