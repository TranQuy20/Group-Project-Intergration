using CEO_Memo.DAL;
using CEO_Memo.Models;
using CEO_Memo.Models.Payroll;
using CEO_Memo.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace CEO_Memo.Controllers
{
    public class AlertsController : System.Web.Http.ApiController
    {
        private readonly HumanContext dbHuman = new HumanContext();
        private readonly PayrollContext dbPayroll = new PayrollContext();

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/alerts")]
        public IHttpActionResult PostAlerts()
        {
            var alerts = new List<string>();
            var month = DateTime.Now;
            var previousMonth = month.AddMonths(-1);

            // ⚠️ 1. Cảnh báo nghỉ phép vượt quá
            var leaveViolations = dbPayroll.Attendances
                .Where(a => a.AttendanceMonth.Month == month.Month &&
                            a.AttendanceMonth.Year == month.Year &&
                            a.LeaveDays > 5)
                .ToList();

            foreach (var a in leaveViolations)
            {
                var emp = dbHuman.Employees.FirstOrDefault(e => e.EmployeeID == a.EmployeeID);
                if (emp != null && !string.IsNullOrEmpty(emp.Email))
                {
                    string subject = "⚠️ Cảnh báo nghỉ phép vượt quá quy định";
                    string body = $"<p>Xin chào {emp.FullName},</p><p>Bạn đã nghỉ phép {a.LeaveDays} ngày trong tháng {month:MM/yyyy}.</p>" +
                                  "<p>Vui lòng tuân thủ quy định nghỉ phép của công ty.</p>";

                    EmailHelper.SendEmail(emp.Email, subject, body);
                    alerts.Add($"Đã gửi cảnh báo nghỉ phép cho {emp.FullName} ({emp.Email})");
                }
            }

            // ⚠️ 2. Cảnh báo biến động lương bất thường (> 5 triệu)
            var currentPayrolls = dbPayroll.Salaries
                .Where(p => p.SalaryMonth.Month == month.Month &&
                            p.SalaryMonth.Year == month.Year)
                .ToList();

            foreach (var p in currentPayrolls)
            {
                var lastMonth = dbPayroll.Salaries.FirstOrDefault(x => x.EmployeeID == p.EmployeeID &&
                    x.SalaryMonth.Month == previousMonth.Month &&
                    x.SalaryMonth.Year == previousMonth.Year);

                if (lastMonth != null && Math.Abs(p.NetSalary - lastMonth.NetSalary) > 5000000)
                {
                    var emp = dbHuman.Employees.FirstOrDefault(e => e.EmployeeID == p.EmployeeID);
                    if (emp != null && !string.IsNullOrEmpty(emp.Email))
                    {
                        string subject = "⚠️ Cảnh báo biến động lương bất thường";
                        string body = $"<p>Chào {emp.FullName},</p><p>Lương thực nhận tháng này chênh lệch hơn 5 triệu so với tháng trước.</p>" +
                                      "<p>Vui lòng kiểm tra lại với phòng nhân sự nếu có sai sót.</p>";

                        EmailHelper.SendEmail(emp.Email, subject, body);
                        alerts.Add($"Đã gửi cảnh báo biến động lương cho {emp.FullName} ({emp.Email})");
                    }
                }
            }

            // 🎉 3. Nhắc nhở kỷ niệm ngày làm việc hôm nay
            var today = DateTime.Today;
            var anniversaries = dbHuman.Employees
                .Where(e => e.HireDate.HasValue &&
                            e.HireDate.Value.Month == today.Month &&
                            e.HireDate.Value.Day == today.Day)
                .ToList();

            foreach (var emp in anniversaries)
            {
                string subject = "🎉 Kỷ niệm ngày vào làm!";
                string body = $"<p>Chào {emp.FullName},</p><p>Hôm nay là kỷ niệm ngày bạn vào làm tại công ty ({emp.HireDate:dd/MM/yyyy}).</p><p>Chúc bạn nhiều thành công!</p>";

                if (!string.IsNullOrEmpty(emp.Email))
                {
                    EmailHelper.SendEmail(emp.Email, subject, body);
                    alerts.Add($"Đã gửi lời chúc kỷ niệm ngày làm việc cho {emp.FullName} ({emp.Email})");
                }
            }

            return Ok(alerts); // Trả về danh sách các cảnh báo đã gửi
        }
    }
}
