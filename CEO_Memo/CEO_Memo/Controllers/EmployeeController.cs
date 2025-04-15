using CEO_Memo.DAL;
using CEO_Memo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity; // ✅ THÊM dòng này
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CEO_Memo.Models.Payroll;
using CEO_Memo.Services;

namespace CEO_Memo.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly HumanContext db = new HumanContext(); // ✅ nên dùng readonly
        private readonly PayrollContext dbPayroll = new PayrollContext(); // ✅ MySQL context


        // Xem danh sách nhân viên
        public ActionResult Index(string search)
        {
            var employees = from e in db.Employees select e;

            if (!string.IsNullOrEmpty(search))
            {
                employees = employees.Where(e =>
                    e.FullName.Contains(search));
            }

            return View(employees.ToList());
        }


        // Thêm mới
        public ActionResult Create()
        {
            ViewBag.Departments = new SelectList(db.Departments, "DepartmentID", "DepartmentName");
            ViewBag.Positions = new SelectList(db.Positions, "PositionID", "PositionName");
            ViewBag.RoleOptions = new SelectList(new[]
    {
        new { Value = "Admin", Text = "Quản trị viên" },
        new { Value = "HR", Text = "Quản lý nhân sự" },
        new { Value = "Payroll", Text = "Quản lý bảng lương" },
        new { Value = "Staff", Text = "Nhân viên" }
    }, "Value", "Text");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // SQL Server
                        employee.CreatedAt = DateTime.Now;
                        db.Employees.Add(employee);
                        db.SaveChanges();

                        // MySQL
                        var payrollEmp = new EmployeePayroll
                        {
                            EmployeeID = employee.EmployeeID,
                            FullName = employee.FullName,
                            DepartmentID = employee.DepartmentID,
                            PositionID = employee.PositionID,
                            Status = employee.Status
                        };
                        dbPayroll.Employees.Add(payrollEmp);
                        dbPayroll.SaveChanges();
                        SyncService.SyncEmployeeFromSqlToMySql(employee, dbPayroll);
                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        var errorMessage = ex.InnerException?.InnerException?.Message
                                        ?? ex.InnerException?.Message
                                        ?? ex.Message;

                        System.Diagnostics.Debug.WriteLine("💥 Lỗi chi tiết: " + errorMessage); // in ra Output

                        ModelState.AddModelError("", "Lỗi khi thêm nhân viên: " + errorMessage);
                    }

                }
            }

            ViewBag.Departments = new SelectList(db.Departments, "DepartmentID", "DepartmentName", employee.DepartmentID);
            ViewBag.Positions = new SelectList(db.Positions, "PositionID", "PositionName", employee.PositionID);
            ViewBag.RoleOptions = new SelectList(new[]
{
    new { Value = "Admin", Text = "Quản trị viên" },
    new { Value = "HR", Text = "Quản lý nhân sự" },
    new { Value = "Payroll", Text = "Quản lý bảng lương" },
    new { Value = "Staff", Text = "Nhân viên" }
}, "Value", "Text", employee.Role); // chọn lại giá trị đã chọn trước đó
            return View(employee);
        }



        // Cập nhật
        public ActionResult Edit(int id)
        {
            var emp = db.Employees.Find(id);
            if (emp == null)
                return HttpNotFound();

            ViewBag.Departments = new SelectList(db.Departments, "DepartmentID", "DepartmentName", emp.DepartmentID);
            ViewBag.Positions = new SelectList(db.Positions, "PositionID", "PositionName", emp.PositionID);

            return View(emp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee emp)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Cập nhật SQL Server
                        emp.UpdatedAt = DateTime.Now;
                        db.Entry(emp).State = EntityState.Modified;
                        db.SaveChanges();

                        // Cập nhật MySQL
                        var mysqlEmp = dbPayroll.Employees.Find(emp.EmployeeID);
                        if (mysqlEmp != null)
                        {
                            mysqlEmp.FullName = emp.FullName;
                            mysqlEmp.DepartmentID = emp.DepartmentID;
                            mysqlEmp.PositionID = emp.PositionID;
                            mysqlEmp.Status = emp.Status;
                            dbPayroll.SaveChanges();
                        }
                        SyncService.SyncEmployeeFromSqlToMySql(emp, dbPayroll);
                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
                    }
                }
            }

            ViewBag.Departments = new SelectList(db.Departments, "DepartmentID", "DepartmentName", emp.DepartmentID);
            ViewBag.Positions = new SelectList(db.Positions, "PositionID", "PositionName", emp.PositionID);
            return View(emp);
        }


        // Xóa
        public ActionResult Delete(int id)
        {
            var emp = db.Employees.Find(id);
            if (emp == null)
                return HttpNotFound();
            return View(emp);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var emp = db.Employees.Find(id);
            if (emp != null)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Kiểm tra điều kiện ràng buộc ở PAYROLL (ví dụ có bảng lương chưa?)
                        var hasSalary = dbPayroll.Salaries.Any(s => s.EmployeeID == id);
                        if (hasSalary)
                        {
                            ModelState.AddModelError("", "Không thể xoá nhân viên có dữ liệu bảng lương.");
                            return View(emp);
                        }

                        // Xoá SQL Server
                        db.Employees.Remove(emp);
                        db.SaveChanges();

                        // Xoá MySQL
                        var mysqlEmp = dbPayroll.Employees.Find(id);
                        if (mysqlEmp != null)
                        {
                            dbPayroll.Employees.Remove(mysqlEmp);
                            dbPayroll.SaveChanges();
                        }

                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Lỗi xoá nhân viên: " + ex.Message);
                        return View(emp);
                    }
                }
            }

            return RedirectToAction("Index");
        }



    }
}
