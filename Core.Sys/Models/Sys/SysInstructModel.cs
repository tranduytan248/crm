using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Sys.Models.Sys
{
    public class SysInstructModel : BaseModel
    {
        public int InstructID { get; set; }
        [CustomDisplayName("Instruct_Label_Name")]
        public string InstructName { get; set; }
        [CustomDisplayName("Instruct_Label_Content")]
        public string Content { get; set; }
        [CustomDisplayName("Menu_Label_IsShow")]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        [CustomDisplayName("Instruct_Label_InstructParent")]
        public int? InstructParentID { get; set; }
        [CustomDisplayName("Instruct_Label_PositionShow")]
        public int PositionShow { get; set; }
        public int Level { get; set; }
        public bool HasChild { get; set; }
    }

    public class InstructTreeModel
    {
        public int InstructID { get; set; }
        public string InstructName { get; set; }
        public string Content { get; set; }
        public int? InstructParentID { get; set; }
        public List<InstructTreeModel> Children { get; set; } = new List<InstructTreeModel>();
    }

    public class InstructSearchModel
    {
        public int Keyword { get; set; }
    }
}
