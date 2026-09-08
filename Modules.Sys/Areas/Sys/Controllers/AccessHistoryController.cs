using Core.Log.Caches;
using Core.Sys.BaseApp;
using System;
using System.Web.Mvc;
using Core.Log.Models;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;

namespace Modules.Sys.Areas.Sys.Controllers
{
    public class AccessHistoryController : AppController
    {
        private readonly AccessHistoryCache _accessHistoryCache = new AccessHistoryCache();

        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        //Sys/AccessHistory
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Tìm kiếm lịch sử truy cập
        /// </summary>
        /// <returns></returns>
        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get(AccessHistorySearchModel searchModel)
        {
            var search = Request.Form.GetValues("search[value]")?[0];
            var draw = Request.Form.GetValues("draw")?[0];
            var order = Request.Form.GetValues("order[0][column]")?[0];
            var orderDir = Request.Form.GetValues("order[0][dir]")?[0];
            var startRec = Convert.ToInt32(Request.Form.GetValues("start")?[0]);
            var pageSize = Convert.ToInt32(Request.Form.GetValues("length")?[0]);

            var dataSearch = new BaseSearchModel
            {
                Search = string.IsNullOrEmpty(search) ? null : search,
                Order = order,
                OrderDir = orderDir,
                StartIndex = startRec,
                PageSize = pageSize
            };
            var data = _accessHistoryCache.Get(out var total, searchModel, dataSearch);

            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }
    }
}