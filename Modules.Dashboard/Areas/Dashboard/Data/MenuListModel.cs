using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Modules.Dashboard.Areas.Dashboard.Data
{
    // danh sách menu 
    public class MenuListModel
    {
        public List<MenuItemModel> Items { get; set; }
    }

    // menu item
    public class MenuItemModel
    {
        public int MenuId { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }

    }
}