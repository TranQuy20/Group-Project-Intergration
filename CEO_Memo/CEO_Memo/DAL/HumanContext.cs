using CEO_Memo.Models;
using System.Data.Entity;

namespace CEO_Memo.DAL
{
    public class HumanContext : DbContext
    {
        public HumanContext() : base("HumanConnection") { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Dividend> Dividends { get; set; }

    }
}
