using System.ComponentModel;

namespace TSFramework.Libs.Enums
{
    public enum EnumProcessType
    {
        [Description("Add")] Add = 1,
        [Description("Edit")] Edit = 2,
        [Description("Delete")] Delete = 3,
        [Description("Common_DataExisted")] DataExisted = 4,
        [Description("Common_DataNotExist")] DataNotExist = 5,
        [Description("Common_NonFormat")] NonFormat = 6,
        [Description("Confirm")] Confirm = 7,
        [Description("Destroy")] Destroy = 8,
        [Description("Recreate")] Recreate = 9,
        [Description("Return")] Return = 10,
        [Description("Create")] Create = 11,
        [Description("Convert")] Convert = 12,
        [Description("Common_DataUsed")] DataUsed = 13,
    }
}