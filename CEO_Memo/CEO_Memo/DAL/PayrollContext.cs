using CEO_Memo.Models;
using System.Data.Entity;
using CEO_Memo.Models.Payroll;

namespace CEO_Memo.DAL
{
    public class PayrollContext : DbContext
    {
        public PayrollContext() : base("PayrollConnection") { }

        public DbSet<EmployeePayroll> Employees { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Department> Departments { get; set; }
    }
}
