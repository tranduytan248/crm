using System;
using System.Collections.Generic;
using System.Linq;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_ExchangeHistoryFilePathModel : BaseSearchModel
    {
        //FilePathID 
        public int FilePathID { get; set; }
        //ExchangeHistoryID 
        public int ExchangeHistoryID { get; set; }
        //FilePath 
        public string FilePath { get; set; }
        //IsDeleted 
        public bool IsDeleted { get; set; }

        public int BusinessOpportunityID { get; set; }
    }

    public class RM_ExchangeHistoryFilePathSearchModel
    {
        //ExchangeHistoryID 
        public int ExchangeHistoryID { get; set; }
        public int BusinessOpportunityID { get; set; }

    }
}
