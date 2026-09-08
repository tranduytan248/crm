using TSFramework.Libs.Models.Base;

namespace Core.Sys.Models.Sys
{
    public class SysContentPanelModel : BaseModel
    {
        public int ContentPanelId { get; set; }
        public string ContentPanelName { get; set; }
        public string Note { get; set; }
        public int LayoutId { get; set; }
        public bool Deleted { get; set; }
        public int WidthCol { get; set; } = 1;
        public new int? TotalRow { get; set; } = 0;
    }
}