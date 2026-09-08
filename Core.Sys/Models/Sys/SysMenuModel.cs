using System.Collections.Generic;
using System.Web.UI.WebControls;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Sys.Models.Sys
{
    public class SysMenuModel : BaseModel
    {
        public string ModuleName { get; set; }

        public int MenuId { get; set; }

        [CustomRequired]
        [CustomDisplayName("Menu_Label_Name")]
        public string Name { get; set; }

        public int? Position { get; set; } = 1;
        public int? LevelMenu { get; set; } = 1;
        public string Depth { get; set; }
        public int? ParentId { get; set; }

        [CustomRequired]
        [CustomDisplayName("Menu_Label_Link")]
        public string Link { get; set; }

        public string Icon { get; set; } = "fas fa-heart";
        public int? FunctionActionId { get; set; }

        [CustomDisplayName("Menu_Label_IsShow")]
        public bool IsShow { get; set; } = true;
        public string FunctionName { get; set; }
        public new int? TotalRow { get; set; } = 0;

        [CustomDisplayName("Menu_Label_UseModal")]
        public bool UseModal { get; set; } = false;

        [CustomDisplayName("Menu_Label_ModalId")]
        public string ModalId { get; set; }
        public string TitleView { get; set; }
        public List<ListItem> FunctionActions { get; set; }

        public List<ListItem> ParentMenus { get; set; }
    }
}