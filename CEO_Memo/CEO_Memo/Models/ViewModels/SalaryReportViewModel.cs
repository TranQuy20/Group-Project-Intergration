using System;
using System.Collections.Generic;
using CEO_Memo.Models;

namespace CEO_Memo.Models.ViewModels
{
    public class SalaryReportViewModel
    {
        public string Month { get; set; }
        public int TotalPayrolls { get; set; }
        public decimal TotalNetSalary { get; set; }
        public decimal TotalAllowance { get; set; }
        public decimal TotalDeduction { get; set; }

        public List<Salary> PayrollDetails { get; set; }  // Chi tiết các bảng lương
    }
}
