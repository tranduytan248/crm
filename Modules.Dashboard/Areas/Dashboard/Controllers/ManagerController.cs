using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;
using Modules.Dashboard.Areas.Dashboard.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Extensions;
using TSFramework.Libs.Processors;

namespace Modules.Dashboard.Areas.Dashboard.Controllers
{
    public class ManagerController : AppController
    {
        private readonly SysMenuCache _apiMenu = new SysMenuCache();
        private readonly string _dashboardName = AppProcessor.Messagor.GetMessage("Dashboard_Title");
        // GET: Dashboard/Manager
        public ActionResult Index(int menu = 0)
        {
            ViewBag.IDMenu = menu;
            return View();
        }

        //Lấy danh sách các menu theo quyền user
        [AjaxOnly]
        [HttpPost]
        public ActionResult Get(int id)
        {
            // lấy danh sách menu gốc
            var data = _apiMenu.GetByUserName(User?.UserName);
            var listMenu = data.Where(m => id != 0 ? m.ParentId == id : m.ParentId == null);        
            List<MenuItemModel> menuItems = listMenu.Select(m => new MenuItemModel
            { 
                MenuId = m.MenuId, 
                Icon= m.Icon, 
                Name = m.Name 
            }).ToList();
            MenuListModel listMenuView = new MenuListModel()
            {
                Items = menuItems
            };
            var menuById = _apiMenu.GetById(id);

            return Json( new { 
                data = listMenu != null ? PartialView("_MenuList", listMenuView).RenderToString() : null, 
                url = listMenu.FirstOrDefault() == null && id != 0 ? menuById.Link : "", 
                leaf = listMenu.FirstOrDefault() == null && id != 0 ? false : true, 
                depth = id != 0 ? menuById.Depth : "", 
                titleView = id != 0 ? menuById.TitleView : "" 
            }, JsonRequestBehavior.AllowGet);
        }
    }
}