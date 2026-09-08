using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using TSFramework.Core.Enums;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Helpers;
using TSFramework.Libs.Processors;

namespace Core.Sys.Models.Sys
{
    public class SysPermissionAPIModel
    {
        public int? UserID { get; set; }
        public string UserName { get; set; }

        [CustomRequired]
        [CustomDisplayName("PermissionAPI_ApplyFor")]
        public string ApplyFor { get; set; }

        public List<ListItem> ListApplyFor
        {
            get
            {
                return Enum.GetValues(typeof(EnumApplyFor))
                    .Cast<EnumApplyFor>()
                    .Where(t => (int)t >= 0)
                    .Select(t => new ListItem
                    {
                        Value = EnumHelper.GetDescription(t),
                        Text = AppProcessor.Messagor.GetMessage(t.ToString())
                    }).ToList();
            }
        }
        public SysUserModel ThongTinUser { get; set; } = new SysUserModel();
    }
}
