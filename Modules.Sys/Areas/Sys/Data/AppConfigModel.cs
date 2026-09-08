using TSFramework.Libs.Attributes;

namespace Modules.Sys.Areas.Sys.Data
{
    public class AppSettingModel
    {
        [CustomRequired]
        [CustomDisplayName("AppSetting_Label_KeyName")]
        public string AppKey { get; set; }

        [CustomRequired]
        [CustomDisplayName("AppSetting_Label_Value")]
        public string AppValue { get; set; }
    }
}