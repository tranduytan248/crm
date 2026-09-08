using System;
using System.Collections.Generic;

namespace Core.RM.Models
{
    public class RM_CustomerImportRowModel
    {
        public int RowNumber { get; set; }
        public string CustomerName { get; set; }
        public string ShortName { get; set; }
        public string TaxCode { get; set; }
        public string AddressCus { get; set; }
        public string CompanyType { get; set; }
        public DateTime? EstablishmentDate { get; set; }
        public string Website { get; set; }
        public decimal CharterCapital { get; set; }  
        public string Province { get; set; }
        public string Ward { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public byte CustomerTypeID { get; set; }
        public string CustomerTypeName { get; set; }
        public byte CustomerStatusID { get; set; }
        public string StatusName { get; set; }
        public string StatusClass { get; set; }

        // --- Kết quả validate ---
        public List<string> Errors { get; set; } = new List<string>();
        public bool IsValid => Errors == null || Errors.Count == 0;
        public string ErrorMessage => Errors != null ? string.Join("; ", Errors) : "";
    }
}