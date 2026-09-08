using TSFramework.Libs.Attributes;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_BillingCyclesModel : BaseModel
    {
        public int BillingCycleID { get; set; }
        [CustomRequired]
        [CustomDisplayName("CycleName_Label")]
        public string CycleName { get; set; }
        //Islimited 
        [CustomDisplayName("Islimited_Label")]
        public bool Islimited { get; set; } = true;
        //Quantity 
        [CustomRequired]
        [CustomDisplayName("Quantity_Label")]
        public int Quantity { get; set; }
        [CustomRequired]
        [CustomDisplayName("CycleType_Label")]
        public string CycleType { get; set; }
    }
}
