using Core.Cate.Models;
using System.Linq;
using System.Data;
using Core.Cate.Biz;
using Core.Cate.Caches;
using System.Web.Mvc;
using Core.Sys.BaseApp;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using System;


namespace Modules.Cate.Areas.Cate.Controllers
{
    public class OpportunityProjectWithoutPGPController : AppController
    {
        private readonly OpportunityProjectWithoutPGPCache _opportunityProjectWithoutPGPCache;
        public OpportunityProjectWithoutPGPController()
        {
            _opportunityProjectWithoutPGPCache = new OpportunityProjectWithoutPGPCache();
        }

        public ActionResult Index()
        {
            var model = new OpportunityProjectWithoutPGSearchPModel
            {
                Year = DateTime.Today.Year,
            };
            return View(model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get(OpportunityProjectWithoutPGSearchPModel searchModel)
        {
            var draw = Request.Form.GetValues("draw")?[0];
            var order = Request.Form.GetValues("order[0][column]")?[0];
            var orderDir = Request.Form.GetValues("order[0][dir]")?[0];
            var startRec = Convert.ToInt32(Request.Form.GetValues("start")?[0]);
            var pageSize = Convert.ToInt32(Request.Form.GetValues("length")?[0]);
            var filterKeyword = Request.Form.GetValues("search[value]")?[0];
            var columns = new[]
            {
                "",
                "ReportID",
                "ReportName",
                "StatusName",
                "ReportType", 
                "AM",        
                "CreatedDate"
            };
            var orderColumn = columns[Convert.ToInt32(order)];

            var dataSearch = new OpportunityProjectWithoutPGSearchPModel
            {
                Search = string.IsNullOrEmpty(filterKeyword) ? null : filterKeyword,
                Order = orderColumn,
                OrderDir = orderDir,
                StartIndex = startRec,
                PageSize = pageSize,
            };

            var data = _opportunityProjectWithoutPGPCache.Get(out var total, searchModel, dataSearch);
            return Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);

        }
    }
}
