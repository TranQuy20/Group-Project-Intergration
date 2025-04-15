using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEO_Memo.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalPositions { get; set; }
        public decimal TotalNetSalaryMonth { get; set; }
    }

}