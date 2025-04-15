using CEO_Memo.DAL;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CEO_Memo.Controllers
{
    public class HomeController : Controller
    {
        private HumanContext dbHuman = new HumanContext();
        private PayrollContext dbPayroll = new PayrollContext();

        [Authorize]
        public ActionResult Dashboard()
        {
            var role = (Session["UserRole"]?.ToString() ?? "").Trim().ToLower();

            if (role == "staff")
            {
                return RedirectToAction("StaffDashboard");
            }

            var month = DateTime.Now;

            var model = new Models.ViewModels.DashboardViewModel
            {
                TotalEmployees = dbHuman.Employees.Count(),
                TotalDepartments = dbHuman.Departments.Count(),
                TotalPositions = dbHuman.Positions.Count(),
                TotalNetSalaryMonth = dbPayroll.Salaries
                    .Where(p => p.SalaryMonth.Month == month.Month && p.SalaryMonth.Year == month.Year)
                    .Sum(p => (decimal?)p.NetSalary) ?? 0
            };

            return View(model);
        }

        [Authorize]
        public ActionResult StaffDashboard()
        {
            var userId = Convert.ToInt32(Session["UserID"]);
            var emp = dbHuman.Employees
                        .Include("Department")
                        .Include("Position")
                        .FirstOrDefault(e => e.EmployeeID == userId);

            var salaryHistory = dbPayroll.Salaries
                .Where(s => s.EmployeeID == userId)
                .OrderByDescending(s => s.SalaryMonth)
                .ToList();

            ViewBag.Employee = emp;
            ViewBag.SalaryHistory = salaryHistory;

            return View(); // Tìm Views/Home/StaffDashboard.cshtml
        }
    }
}
