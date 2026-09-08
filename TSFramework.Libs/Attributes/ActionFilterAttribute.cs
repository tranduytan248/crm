using System.Web.Mvc;
using TSFramework.Libs.Enums;

namespace TSFramework.Libs.Attributes
{
    public class AllowAnyPermissionAttribute : FilterAttribute
    {
    }

    public class ActionTypeAttribute : FilterAttribute
    {
        public EnumActionType Type { get; set; }
    }

    public class ApiActionAttribute : FilterAttribute
    {
    }
}