using System;
using System.Collections.Generic;

namespace Core.Cate.Models
{
    public class RM_ContactPersonsImportRowModel
    {
        public int RowNumber { get; set; }
        public string FullName { get; set; }
        public int? Gender { get; set; }
        public string GenderName { get; set; }
        public string Position { get; set; }
        public string WorkUnit { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Zalo { get; set; }
        public string Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string CustomerShortName { get; set; } 
        public string CustomerName { get; set; }  
        public string Note { get; set; }
        public int Status { get; set; }
        public List<string> Errors { get; set; }
    }
}