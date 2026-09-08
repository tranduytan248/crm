using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Models.Base;

namespace Core.Sys.Models.Sys
{
    public class SysMenuViewModel : BaseModel
    {
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public string Name { get; set; }
        public int Position { get; set; } = 1;
        public int LevelMenu { get; set; } = 1;
        public string Depth { get; set; }
        public string Link { get; set; }
        public string Icon { get; set; }
        public int FunctionActionId { get; set; }
        public bool UseModal { get; set; } = false;
        public string ModalId { get; set; }
        public string TitleView { get; set; }

        public new int? TotalRow { get; set; } = 0;

        public List<SysMenuViewModel> Childs { get; set; }
    }
}
