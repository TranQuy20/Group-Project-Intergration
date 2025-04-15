using CEO_Memo.DAL;
using CEO_Memo.Models;
using CEO_Memo.Models.Payroll;
using System;

namespace CEO_Memo.Services
{
    public static class SyncService
    {
        // Đồng bộ từ SQL Server sang MySQL
        public static void SyncEmployeeFromSqlToMySql(Employee emp, PayrollContext dbPayroll)
        {
            var existing = dbPayroll.Employees.Find(emp.EmployeeID);
            if (existing == null)
            {
                dbPayroll.Employees.Add(new EmployeePayroll
                {
                    EmployeeID = emp.EmployeeID,
                    FullName = emp.FullName,
                    DepartmentID = emp.DepartmentID,
                    PositionID = emp.PositionID,
                    Status = emp.Status
                });
            }
            else
            {
                existing.FullName = emp.FullName;
                existing.DepartmentID = emp.DepartmentID;
                existing.PositionID = emp.PositionID;
                existing.Status = emp.Status;
            }
            dbPayroll.SaveChanges();
        }

        // Đồng bộ từ MySQL sang SQL Server
        public static void SyncEmployeeFromMySqlToSql(EmployeePayroll emp, HumanContext dbHuman)
        {
            var existing = dbHuman.Employees.Find(emp.EmployeeID);
            if (existing == null)
            {
                dbHuman.Employees.Add(new Employee
                {
                    EmployeeID = emp.EmployeeID,
                    FullName = emp.FullName,
                    DepartmentID = emp.DepartmentID,
                    PositionID = emp.PositionID,
                    Status = emp.Status,
                    CreatedAt = DateTime.Now,
                    Role = "Staff"
                });
            }
            else
            {
                existing.FullName = emp.FullName;
                existing.DepartmentID = emp.DepartmentID;
                existing.PositionID = emp.PositionID;
                existing.Status = emp.Status;
                existing.UpdatedAt = DateTime.Now;
            }
            dbHuman.SaveChanges();
        }
    }
}
