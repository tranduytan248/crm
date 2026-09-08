using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.BaseApps;
using TSFramework.Libs.Models.Base;

namespace Core.Cate.Models
{
    public class RM_CustomerContactModel : BaseModel
    {
        public int CustomerContactID { get; set; }
        [CustomRequired]
        public int CustomerID { get; set; }
        public int ContactPersonID { get; set; }
        [CustomDisplayName("ContactPersonal_Position_Label")]
        public string Position { get; set; }
        [CustomDisplayName("ContactPersonal_Note_Label")]
        public string Note { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // ====== từ ContactPerson ======
        [CustomDisplayName("ContactPersonal_FullName_Label")]
        public string FullName { get; set; }
        [CustomDisplayName("ContactPersonal_Phone_Label")]
        public string Phone { get; set; }
        [CustomDisplayName("ContactPersonal_Mobile_Label")]
        public string Mobile { get; set; }
        [CustomDisplayName("ContactPersonal_Email_Label")]
        public string Email { get; set; }
        [CustomDisplayName("ContactPersonal_Zalo_Label")]
        public string Zalo { get; set; }
        [CustomDisplayName("ContactPersonal_Address_Label")]
        public string Address { get; set; }
        public int? Gender { get; set; }
        public int TempID { get; set; }

    }
}
