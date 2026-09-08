using System;
using System.ComponentModel;

namespace TSFramework.Libs.Enums
{
    [Flags]
    public enum EnumActionType
    {
        [Description("View")] View = 1,
        [Description("Create")] Create = 2,
        [Description("Edit")] Edit = 4,
        [Description("Delete")] Delete = 8,
        [Description("All")] All = 15
    }
}