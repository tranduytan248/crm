using System.ComponentModel;

namespace TSFramework.Libs.Enums
{
    public enum EnumReponseStatus
    {
        [Description("Error")] Error = 0,
        [Description("Success")] Success = 1,
        [Description("AccessDenied")] AccessDenied = 2,
        [Description("AuthenticationDenied")] AuthenticationDenied = 3,
        [Description("Existed")] Existed = -9
    }
}