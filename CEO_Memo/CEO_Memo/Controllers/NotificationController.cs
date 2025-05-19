using System;
using System.Linq;
using System.Web.Mvc;
using CEO_Memo.Models;
using CEO_Memo.Models.ViewModels;
using CEO_Memo.DAL;
using CEO_Memo.Helpers; // Import EmailHelper
using System.Data.Entity;  // Thêm dòng này vào đầu controller của bạn
using System.Collections.Generic;  // Đảm bảo thêm dòng này để sử dụng List<>
using CEO_Memo.Filters; // Đảm bảo bạn đã thêm namespace chứa AuthorizeRoles


namespace CEO_Memo.Controllers
{
    [AuthorizeRoles("Admin")]
    public class NotificationController : Controller
    {

        private PayrollContext dbPayroll = new PayrollContext();  // Kết nối với PayrollContext nếu bạn đã cấu hình đúng
        private HumanContext dbHuman = new HumanContext();
        // Action để hiển thị bảng lương cho tất cả nhân viên trước khi gửi
        public ActionResult SendMonthlyPayroll()
        {
            var currentMonth = DateTime.Now;
            var salaries = dbPayroll.Salaries
                .Where(s => s.SalaryMonth.Month == currentMonth.Month && s.SalaryMonth.Year == currentMonth.Year)
                .ToList(); // Lấy tất cả bảng lương trong tháng này

            // Kiểm tra nếu có bảng lương
            if (salaries.Count > 0)
            {
                // Tạo ViewModel để gửi dữ liệu bảng lương
                var totalPayrolls = salaries.Count();
                var totalNetSalary = salaries.Sum(s => s.NetSalary);
                var totalAllowance = salaries.Sum(s => s.Bonus);
                var totalDeduction = salaries.Sum(s => s.Deductions);

                var payrollReport = new SalaryReportViewModel
                {
                    Month = currentMonth.ToString("MM/yyyy"),
                    TotalPayrolls = totalPayrolls,
                    TotalNetSalary = totalNetSalary,
                    TotalAllowance = totalAllowance,
                    TotalDeduction = totalDeduction,
                    PayrollDetails = salaries
                };

                return View(payrollReport); // Trả về view với bảng lương
            }
            else
            {
                ViewBag.Message = "Không có bảng lương cho tháng này để gửi.";
                return View();
            }
        }

        // Action để gửi lương cho tất cả nhân viên
        [HttpPost]
        public ActionResult SendMonthlyPayroll(SalaryReportViewModel model)
        {
            var currentMonth = DateTime.Now;
            var salaries = dbPayroll.Salaries
                .Where(s => s.SalaryMonth.Month == currentMonth.Month && s.SalaryMonth.Year == currentMonth.Year)
                .ToList(); // Lấy tất cả bảng lương trong tháng này

            // Gửi email cho tất cả nhân viên
            foreach (var salary in salaries)
            {
                var employeeEmail = GetEmployeeEmail(salary.EmployeeID);
                if (string.IsNullOrEmpty(employeeEmail))
                {
                    continue; // Nếu không có email thì bỏ qua
                }

                // Tạo chủ đề và nội dung email
                string subject = "Báo cáo lương tháng " + currentMonth.ToString("MM/yyyy");
                string body = $"Kính gửi, nhân viên {salary.EmployeeID},\n\n" +
                              $"Bảng lương tháng {salary.SalaryMonth.ToString("MM/yyyy")} của bạn như sau:\n" +
                              $"Lương cơ bản: {salary.BaseSalary:C}\n" +
                              $"Phụ cấp: {salary.Bonus:C}\n" +
                              $"Khấu trừ: {salary.Deductions:C}\n" +
                              $"Lương thực lĩnh: {salary.NetSalary:C}\n\n" +
                              "Trân trọng,\n" +
                              "Phòng Nhân Sự";

                // Gửi email cho nhân viên
                try
                {
                    EmailHelper.SendEmail(employeeEmail, subject, body);  // Gửi email cho nhân viên
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email to {employeeEmail}: {ex.Message}");
                }
            }

            // Sau khi gửi email, thông báo thành công
            ViewBag.Message = "Lương đã được gửi cho tất cả nhân viên.";

            return View(model); // Trả về view với thông báo
        }
        

        // Phương thức lấy email của nhân viên từ bảng Employees
        private string GetEmployeeEmail(int? employeeId)
        {
            using (var humanDb = new HumanContext())
            {
                var employee = humanDb.Employees.FirstOrDefault(e => e.EmployeeID == employeeId);
                return employee?.Email;  // Trả về email của nhân viên nếu tồn tại
            }
        }

        public ActionResult WorkAnniversary()
        {
            var today = DateTime.Today;

            // Lấy danh sách nhân viên có kỷ niệm ngày làm việc hôm nay
            var anniversaries = dbHuman.Employees
                .Where(e => e.HireDate.HasValue && e.HireDate.Value.Month == today.Month && e.HireDate.Value.Day == today.Day)
                .ToList();

            if (anniversaries.Count == 0)
            {
                ViewBag.Message = "Không có nhân viên nào có kỷ niệm ngày vào làm hôm nay.";
            }

            ViewBag.Anniversaries = anniversaries;
            return View();
        }
        [HttpPost]
        public ActionResult SendAnniversaryEmails()
        {
            var today = DateTime.Today;

            // Lấy danh sách nhân viên có kỷ niệm ngày làm việc hôm nay
            var anniversaries = dbHuman.Employees
                .Where(e => e.HireDate.HasValue && e.HireDate.Value.Month == today.Month && e.HireDate.Value.Day == today.Day)
                .ToList();

            // Kiểm tra nếu không có nhân viên nào
            if (anniversaries.Count > 0)
            {
                foreach (var emp in anniversaries)
                {
                    string subject = "🎉 Chúc mừng kỷ niệm ngày vào làm tại công ty!";
                    string body = $@"
            <p>Kính gửi anh/chị {emp.FullName},</p>
            <p>Hôm nay là một cột mốc quan trọng trong hành trình của bạn tại công ty – kỷ niệm ngày bạn gia nhập công ty (ngày {emp.HireDate:dd/MM/yyyy}).</p>
            <p>Chúng tôi xin gửi lời chúc mừng nồng nhiệt nhất đến bạn vì những đóng góp tuyệt vời trong suốt thời gian qua. Sự cống hiến và tâm huyết của bạn đã giúp công ty đạt được nhiều thành công đáng kể.</p>
            <p>Nhân dịp này, chúng tôi muốn bày tỏ lòng biết ơn đối với những nỗ lực và sự sáng tạo của bạn. Hy vọng rằng trong những năm tiếp theo, bạn sẽ tiếp tục phát triển cùng công ty, đạt được những mục tiêu cá nhân và cùng chúng tôi xây dựng một môi trường làm việc ngày càng tốt đẹp hơn.</p>
            <p>Chúc bạn một ngày kỷ niệm đầy niềm vui và tiếp tục gặt hái thành công trên con đường sự nghiệp tại công ty.</p>
            <p>Trân trọng, <br>Ban lãnh đạo công ty</p>
            ";

                    // Kiểm tra email và gửi thông báo nếu có email
                    if (!string.IsNullOrEmpty(emp.Email))
                    {
                        try
                        {
                            EmailHelper.SendEmail(emp.Email, subject, body);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending email to {emp.Email}: {ex.Message}");
                        }
                    }
                }

                ViewBag.Message = "Lời chúc kỷ niệm đã được gửi cho các nhân viên.";
            }
            else
            {
                ViewBag.Message = "Không có nhân viên nào có kỷ niệm ngày vào làm hôm nay.";
            }

            return RedirectToAction("WorkAnniversary"); // Redirect lại trang để hiển thị thông báo
        }
        public ActionResult LeaveViolation()
        {
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;

            // Lấy danh sách các nhân viên có số ngày nghỉ phép vượt quá quy định từ bảng Payroll
            var violations = dbPayroll.Attendances
                .Where(a => a.AttendanceMonth.Month == month && a.AttendanceMonth.Year == year && a.LeaveDays > 5)
                .ToList();

            // Truyền danh sách vi phạm vào ViewBag.Violations
            ViewBag.Violations = violations;

            // Truyền vào ViewBag.Message để hiển thị thông báo
            ViewBag.Message = violations.Count > 0 ? "Có nhân viên vi phạm nghỉ phép trong tháng này." : "Không có nhân viên nào vi phạm nghỉ phép trong tháng này.";

            return View();  // Trả về view với danh sách vi phạm
        }


        // Action để kiểm tra nghỉ phép vượt quá và gửi thông báo qua email
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendViolationEmail()
        {
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;

            // Lấy danh sách các nhân viên có số ngày nghỉ phép vượt quá quy định (ví dụ: > 5 ngày)
            var violations = dbPayroll.Attendances
                .Where(a => a.AttendanceMonth.Month == month && a.AttendanceMonth.Year == year && a.LeaveDays > 5)
                .ToList();

            if (violations.Count > 0)
            {
                foreach (var violation in violations)
                {
                    var emp = dbHuman.Employees.FirstOrDefault(e => e.EmployeeID == violation.EmployeeID);
                    if (emp != null && !string.IsNullOrEmpty(emp.Email))
                    {
                        // Chủ đề và nội dung email
                        string subject = "⚠️ Cảnh báo vi phạm nghỉ phép!";
                        string body = $@"
                <p>Kính gửi anh/chị {emp.FullName},</p>
                <p>Chúng tôi xin thông báo rằng bạn đã nghỉ phép {violation.LeaveDays} ngày trong tháng {month}/{year}, vượt quá giới hạn quy định.</p>
                <p>Vui lòng tuân thủ chính sách nghỉ phép của công ty. Nếu có bất kỳ thắc mắc nào, vui lòng liên hệ với bộ phận Nhân sự.</p>
                <p>Trân trọng, <br>Ban lãnh đạo</p>";

                        try
                        {
                            EmailHelper.SendEmail(emp.Email, subject, body);  // Gửi email cho nhân viên
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending email to {emp.Email}: {ex.Message}");
                        }
                    }
                }

                // Thông báo thành công
                ViewBag.Message = "Cảnh báo nghỉ phép đã được gửi cho các nhân viên vi phạm.";
            }
            else
            {
                ViewBag.Message = "Không có nhân viên nào vi phạm nghỉ phép trong tháng này.";
            }

            return View("LeaveViolation");  // Quay lại view LeaveViolation sau khi gửi email
        }

        public ActionResult SalaryDiscrepancy()
        {
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;

            // Get salary discrepancies (employee salary difference)
            var salaryDiscrepancies = dbPayroll.Salaries
                .Where(s => s.SalaryMonth.Month == month && s.SalaryMonth.Year == year)
                .Select(s => new
                {
                    s.EmployeeID,
                    EmployeeFullName = s.Employee.FullName,  // Use the employee's full name correctly
                    CurrentSalary = s.NetSalary,
                    PreviousSalary = s.PreviousMonthSalary // Use PreviousMonthSalary instead of PreviousSalary
                })
                .ToList();

            // Filter employees with salary discrepancies greater than 5 million
            var violations = salaryDiscrepancies
                .Where(s => Math.Abs(s.CurrentSalary - s.PreviousSalary) > 5000000m) // Compare with 5 million
                .ToList();

            // Create view models
            var violationsList = violations.Select(v => new SalaryDiscrepancyViewModel
            {
                EmployeeID = v.EmployeeID ?? 0,
                EmployeeName = v.EmployeeFullName,
                CurrentSalary = v.CurrentSalary,
                PreviousSalary = v.PreviousSalary,
                SalaryDifference = Math.Abs(v.CurrentSalary - v.PreviousSalary),
                EmployeeEmail = dbHuman.Employees.FirstOrDefault(e => e.EmployeeID == v.EmployeeID)?.Email
            }).ToList();

            // Pass data to the view
            ViewBag.Violations = violationsList;


            return View();  // Return view with violations list
        }




        // Action để kiểm tra chênh lệch lương và gửi thông báo qua email
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendSalaryDiscrepancyEmails()
        {
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;

            // Lấy danh sách các nhân viên có chênh lệch lương lớn hơn 5 triệu đồng trong tháng này
            var salaryDiscrepancies = dbPayroll.Salaries
                .Where(s => s.SalaryMonth.Month == month && s.SalaryMonth.Year == year)
                .Select(s => new
                {
                    s.EmployeeID,
                    s.Employee.FullName,
                    CurrentSalary = s.NetSalary,
                    PreviousSalary = dbPayroll.Salaries
                        .Where(p => p.EmployeeID == s.EmployeeID && p.SalaryMonth.Month == month - 1 && p.SalaryMonth.Year == year)
                        .Select(p => p.NetSalary)
                        .FirstOrDefault()
                })
                .ToList();

            var violations = salaryDiscrepancies
                .Where(s => Math.Abs(s.CurrentSalary - s.PreviousSalary) > 5000000)
                .ToList();

            if (violations.Count > 0)
            {
                foreach (var violation in violations)
                {
                    var emp = dbHuman.Employees.FirstOrDefault(e => e.EmployeeID == violation.EmployeeID);
                    if (emp != null && !string.IsNullOrEmpty(emp.Email))
                    {
                        // Chủ đề và nội dung email
                        string subject = "⚠️ Cảnh báo: Chênh lệch lương vượt quá 5 triệu đồng!";
                        string body = $@"
                    <p>Kính gửi anh/chị {emp.FullName},</p>
                    <p>Chúng tôi xin thông báo rằng có sự chênh lệch lương giữa tháng {month}/{year} và tháng {month - 1}/{year} của bạn vượt quá mức quy định (5 triệu đồng).</p>
                    <p>Lương tháng này: {violation.CurrentSalary:C}</p>
                    <p>Lương tháng trước: {violation.PreviousSalary:C}</p>
                    <p>Vui lòng liên hệ với bộ phận Nhân sự để giải thích và xử lý vấn đề này.</p>
                    <p>Trân trọng, <br>Ban lãnh đạo</p>";

                        try
                        {
                            EmailHelper.SendEmail(emp.Email, subject, body);  // Gửi email cho nhân viên
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending email to {emp.Email}: {ex.Message}");
                        }
                    }
                }

                // Thông báo thành công
                ViewBag.Message = "Cảnh báo chênh lệch lương đã được gửi cho các nhân viên vi phạm.";
            }
            else
            {
                ViewBag.Message = "Không có nhân viên nào vi phạm chênh lệch lương trong tháng này.";
            }

            return View("SalaryDiscrepancy");  // Quay lại view SalaryDiscrepancy sau khi gửi email
        }


    }
}
