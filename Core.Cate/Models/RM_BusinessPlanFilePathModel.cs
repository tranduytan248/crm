using System;
using System.Collections.Generic;
using System.Linq;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_BusinessPlanFilePathModel : BaseSearchModel
    {
        public int FilePathID { get; set; }
        public int BusinessPlanID { get; set; }
        public string FilePath { get; set; }
    }
}
