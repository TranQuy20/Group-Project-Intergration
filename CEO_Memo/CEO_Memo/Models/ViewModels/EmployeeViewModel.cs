using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEO_Memo.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DepartmentName { get; set; }  
        public string PositionName { get; set; }    
    }

}