using System;
using System.Web;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Sys.Models.Sys
{
    public class SysModuleModel : BaseModel
    {
        public int ModuleId { get; set; }

        [CustomDisplayName("AppModule_Label_ModuleName")]
        [CustomRequired]
        public string ModuleName { get; set; }

        [CustomDisplayName("AppModule_Label_AssemblyName")]
        public string AssemblyName { get; set; }

        [CustomDisplayName("AppModule_Label_ModuleView")]
        public string ModuleView { get; set; }

        [CustomDisplayName("AppModule_Label_DefaultAction")]
        public string DefaultAction { get; set; }

        [CustomRequired]
        [CustomDisplayName("AppModule_Label_Description")]
        public string Description { get; set; }

        [CustomDisplayName("AppModule_Label_MainController")]
        public string MainController { get; set; }

        public string Icon { get; set; } = "fa fa-heart";
        public string Creator { get; set; }
        public DateTime? CreateDated { get; set; }
        public string Updater { get; set; }
        public DateTime? UpdateDated { get; set; }
        public bool Deleted { get; set; }

        [CustomDisplayName("AppModule_Label_ZipFile")]
        public HttpPostedFileBase ModuleFileZip { get; set; }

        public new int? TotalRow { get; set; } = 0;
    }
}