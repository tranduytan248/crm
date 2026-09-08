using System;
using System.Collections.Generic;
using System.Linq;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_BusinessOpportunityFilePathModel : BaseSearchModel
    {
        public int FilePathID { get; set; }
        public int BusinessOpportunityID { get; set; }
        public string FilePath { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class RM_BusinessOpportunityFilePathSearchModel
    {
        public int BusinessOpportunityID { get; set; }
    }
}
