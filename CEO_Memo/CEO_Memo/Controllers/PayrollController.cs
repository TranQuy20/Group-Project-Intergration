using CEO_Memo.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CEO_Memo.Controllers
{
    public class PayrollController : Controller
    {
        private PayrollContext db = new PayrollContext();

        // Xem bảng lương (theo tháng)
        public ActionResult Index()
        {
            var today = DateTime.Today;
            int month = today.Month;
            int year = today.Year;

            var salaries = db.Salaries
                .Where(s => s.SalaryMonth.Month == month && s.SalaryMonth.Year == year)
                .ToList();

            ViewBag.Month = today.ToString("MM/yyyy");
            return View(salaries);
        }



        // Lịch sử lương theo nhân viên
        public ActionResult SalaryHistory(int employeeId)
        {
            var today = DateTime.Today;
            var recentMonths = 3;  // Số tháng gần nhất
            var startDate = today.AddMonths(-recentMonths);  // Tính toán ngày bắt đầu ngoài truy vấn

            var history = db.Salaries
                .Where(s => s.EmployeeID == employeeId && s.SalaryMonth >= startDate)
                .OrderByDescending(s => s.SalaryMonth)
                .ToList();

            ViewBag.EmployeeID = employeeId;
            return View(history);
        }






        // Dữ liệu chấm công
        public ActionResult Attendance(int? employeeId, DateTime? month)
        {
            if (month == null) month = DateTime.Today;

            var data = db.Attendances
                         .Where(a => (employeeId == null || a.EmployeeID == employeeId) &&
                                     a.AttendanceMonth.Month == month.Value.Month &&
                                     a.AttendanceMonth.Year == month.Value.Year)
                         .ToList();

            ViewBag.Month = month.Value.ToString("MM/yyyy");
            return View(data);
        }

    }

}