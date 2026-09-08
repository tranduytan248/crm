using System.Collections.Generic;
using System.IO;

namespace CenIT.Solution.TOC.WebApp.Models
{
    public class SysLogModel
    {
        public SysLogModel()
        {
            ListErrFiles = ListInvLogFiles = ListJobLogFiles = new List<FileInfo>();
        }

        public List<FileInfo> ListErrFiles { get; set; }

        public List<FileInfo> ListInvLogFiles { get; set; }

        public List<FileInfo> ListJobLogFiles { get; set; }
    }
}