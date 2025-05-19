using CEO_Memo.DAL;
using CEO_Memo.Filters;
using CEO_Memo.Models.ViewModels;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace CEO_Memo.Controllers
{
    public class HomeController : Controller
    {
        private HumanContext dbHuman = new HumanContext();
        private PayrollContext dbPayroll = new PayrollContext();


        // Action để hiển thị Dashboard
        [AuthorizeRoles("Admin", "HR", "Payroll")]
        public ActionResult Dashboard()
        {
            // Kiểm tra nếu session không tồn tại (người dùng chưa đăng nhập)
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Auth");  // Nếu không đăng nhập, chuyển hướng về trang login
            }

            // Kiểm tra vai trò người dùng
            var userRole = Session["UserRole"] as string;
            if (userRole == "Staff")  // Cấm nhân viên (Staff) truy cập Dashboard
            {
                return RedirectToAction("AccessDenied", "Home");  // Chuyển hướng tới trang AccessDenied nếu là nhân viên
            }

            // Lấy dữ liệu tổng hợp cho Dashboard
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;

            var model = new DashboardViewModel
            {
                TotalEmployees = dbHuman.Employees.Count(),
                TotalDepartments = dbHuman.Departments.Count(),
                TotalPositions = dbHuman.Positions.Count(),
                TotalNetSalaryMonth = dbPayroll.Salaries
                    .Where(s => s.SalaryMonth.Month == month && s.SalaryMonth.Year == year)
                    .Sum(s => (decimal?)s.NetSalary) ?? 0
            };

            return View(model);  // Trả về view với dữ liệu Dashboard
        }

        // Thông tin cá nhân của nhân viên
        [AuthorizeRoles("Staff")]
        public ActionResult EmployeeInfo(int employeeId)
        {
            // Lấy thông tin nhân viên
            var employee = dbHuman.Employees
                .Include(e => e.Department)  // Nạp thông tin phòng ban
                .Include(e => e.Position)    // Nạp thông tin chức vụ
                .FirstOrDefault(e => e.EmployeeID == employeeId);

            if (employee == null)
            {
                return HttpNotFound("Không tìm thấy thông tin nhân viên.");
            }

            // Lấy lịch sử lương của nhân viên
            var salaryHistory = dbPayroll.Salaries
                .Where(s => s.EmployeeID == employeeId)
                .OrderByDescending(s => s.SalaryMonth)  // Sắp xếp theo tháng lương (từ mới đến cũ)
                .ToList();

            ViewBag.Employee = employee;
            ViewBag.SalaryHistory = salaryHistory;

            return View(employee);  // Trả về view với model là thông tin nhân viên
        }
        public ActionResult AccessDenied()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}
