using System;
using TSFramework.Libs.Attributes;

namespace TSFramework.Libs.Models.Base
{
    public abstract class BaseModel
    {
        public string UpdatedBy { get; set; }

        [CustomDisplayName("Reason_Title")] public virtual string Reason { get; set; }

        public Int64 RowIndex { get; set; } = 0;

        public int? TotalRow { get; set; } = 0;

        public bool CanEdit { get; set; } = true;

        public bool CanDelete { get; set; } = true;
    }
}