using CEO_Memo.DAL;
using CEO_Memo.Models.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CEO_Memo.Controllers
{
    public class ReportController : Controller
    {
        private HumanContext dbHuman = new HumanContext();
        private PayrollContext dbPayroll = new PayrollContext();

        // 1. Báo cáo tổng quan nhân sự
        public ActionResult HRReport()
        {
            var model = new HRReportViewModel
            {
                TotalEmployees = dbHuman.Employees.Count(),
                TotalDepartments = dbHuman.Departments.Count(),
                TotalPositions = dbHuman.Positions.Count()
            };

            return View(model);
        }

        // 2. Báo cáo lương theo tháng
        public ActionResult SalaryReport(DateTime? month)
        {
            if (month == null) month = DateTime.Now;

            var data = dbPayroll.Salaries
                .Where(p => p.SalaryMonth.Month == month.Value.Month && p.SalaryMonth.Year == month.Value.Year)
                .ToList();

            var model = new SalaryReportViewModel
            {
                Month = month.Value.ToString("MM/yyyy"),
                TotalPayrolls = data.Count(),
                TotalNetSalary = data.Sum(x => (decimal?)x.NetSalary) ?? 0,
                TotalAllowance = data.Sum(x => (decimal?)x.Bonus) ?? 0,
                TotalDeduction = data.Sum(x => (decimal?)x.Deductions) ?? 0,
                PayrollDetails = data 
            };

            return View(model);
        }

    }
}
