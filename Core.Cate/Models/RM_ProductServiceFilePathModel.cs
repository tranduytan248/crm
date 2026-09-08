using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_ProductServiceFilePathModel : BaseModel
    {
        public int FilePathID { get; set; }
        public int GroupFilePathID { get; set; }
        public string FilePath { get; set; }
    }
}
