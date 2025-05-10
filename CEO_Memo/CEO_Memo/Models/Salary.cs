using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEO_Memo.Models
{
    [Table("salaries")]
    public class Salary
    {
        [Key]
        public int SalaryID { get; set; }

        public int? EmployeeID { get; set; }

        [DataType(DataType.Date)]
        public DateTime SalaryMonth { get; set; }

        public decimal BaseSalary { get; set; }

        public decimal Bonus { get; set; }

        public decimal Deductions { get; set; }

        public decimal NetSalary { get; set; }
        public decimal PreviousMonthSalary { get; set; }

        public DateTime CreatedAt { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
