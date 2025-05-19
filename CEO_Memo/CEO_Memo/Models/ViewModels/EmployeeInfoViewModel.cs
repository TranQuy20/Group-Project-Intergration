using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEO_Memo.Models.ViewModels
{
	public class EmployeeInfoViewModel
	{
        public Employee Employee { get; set; }
        public List<Salary> SalaryHistory { get; set; }
    }
}