using System.Collections.Generic;
using System;

namespace CEO_Memo.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? HireDate { get; set; }
        public int? DepartmentID { get; set; }
        public int? PositionID { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        // Quan hệ (navigation properties)
        public virtual Department Department { get; set; }
        public virtual Position Position { get; set; }
        public virtual ICollection<Dividend> Dividends { get; set; }
    }
}
