using CEO_Memo.DAL;
using CEO_Memo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static System.Collections.Specialized.BitVector32;
using System.Web.Mvc;
using CEO_Memo.Helpers;

namespace CEO_Memo.Controllers
{
    public class AuthController : Controller
    {
        private HumanContext dbHuman = new HumanContext();  // SQL Server

        // Action GET để hiển thị form đăng nhập
        public ActionResult Login()
        {
            return View();
        }

        // Action POST để xử lý đăng nhập
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra thông tin đăng nhập
                var user = dbHuman.Employees.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);
                if (user != null)
                {
                    // Lưu thông tin vào session sau khi đăng nhập thành công
                    Session["UserID"] = user.EmployeeID;
                    Session["UserName"] = user.FullName;
                    Session["UserRole"] = user.Role; // Lưu vai trò người dùng

                    // Chuyển hướng đến trang thích hợp dựa trên vai trò
                    if (user.Role == "Staff")  // Nếu là nhân viên, chuyển hướng tới trang Index
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else  // Nếu là Admin hoặc các vai trò khác, chuyển hướng tới Dashboard
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }
                }
                else
                {
                    // Hiển thị thông báo nếu đăng nhập không thành công
                    ViewBag.Message = "Sai tên đăng nhập hoặc mật khẩu!";
                }
            }

            return View(model);  // Trả về view đăng nhập nếu có lỗi
        }
        // Action GET để hiển thị form quên mật khẩu
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // Action POST để xử lý yêu cầu quên mật khẩu
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem email có tồn tại trong cơ sở dữ liệu không
                var user = dbHuman.Employees.FirstOrDefault(x => x.Email == model.Email);
                if (user != null)
                {
                    // Tạo mã khôi phục mật khẩu hoặc token và lưu vào cơ sở dữ liệu
                    var resetToken = Guid.NewGuid().ToString();
                    user.ResetPasswordToken = resetToken;
                    dbHuman.SaveChanges();

                    // Tạo liên kết khôi phục mật khẩu
                    var resetLink = Url.Action("ResetPassword", "Auth", new { token = resetToken }, protocol: Request.Url.Scheme);

                    // Gửi email khôi phục mật khẩu
                    string subject = "Khôi phục mật khẩu của bạn";
                    string body = @"
                <html>
                    <head>
                        <style>
                            body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                            .container { background-color: #f4f4f4; padding: 20px; max-width: 600px; margin: 0 auto; }
                            .header { background-color: #007bff; padding: 10px; text-align: center; color: white; font-size: 24px; }
                            .content { background-color: white; padding: 20px; border-radius: 5px; }
                            .button { background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block; }
                            .footer { text-align: center; font-size: 14px; color: #999; margin-top: 20px; }
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                CEO MEMO - Khôi phục mật khẩu
                            </div>
                            <div class='content'>
                                <p>Chào <strong>" + user.FullName + @"</strong>,</p>
                                <p>Chúng tôi đã nhận được yêu cầu khôi phục mật khẩu cho tài khoản của bạn. Nếu bạn không yêu cầu thay đổi mật khẩu, bạn có thể bỏ qua email này.</p>
                                <p>Để thay đổi mật khẩu, vui lòng nhấp vào liên kết dưới đây:</p>
                                <p><a href='" + resetLink + @"' class='button'>Đặt lại mật khẩu</a></p>
                                <p>Liên kết này sẽ hết hạn trong vòng 24 giờ.</p>
                            </div>
                            <div class='footer'>
                                <p>Trân trọng,<br/>Đội ngũ hỗ trợ của CEO MEMO</p>
                            </div>
                        </div>
                    </body>
                </html>";

                    // Gửi email khôi phục mật khẩu
                    EmailHelper.SendEmail(user.Email, subject, body);

                    ViewBag.Message = "Một liên kết khôi phục mật khẩu đã được gửi đến email của bạn.";
                }
                else
                {
                    ViewBag.Message = "Email không tồn tại trong hệ thống.";
                }
            }

            return View(model);
        }


        // Action GET để hiển thị form nhập mật khẩu mới
        public ActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            // Kiểm tra token trong cơ sở dữ liệu
            var user = dbHuman.Employees.FirstOrDefault(x => x.ResetPasswordToken == token);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(new ResetPasswordViewModel { Token = token });
        }

        // Action POST để xử lý yêu cầu thay đổi mật khẩu mới
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = dbHuman.Employees.FirstOrDefault(x => x.ResetPasswordToken == model.Token);
                if (user != null)
                {
                    user.Password = model.NewPassword;
                    user.ResetPasswordToken = null; // Xóa token sau khi đã thay đổi mật khẩu
                    dbHuman.SaveChanges();

                    return RedirectToAction("Login");
                }
            }
            return View(model);
        }

        // Action để đăng xuất
        public ActionResult Logout()
        {
            Session.Clear();  // Xóa session khi đăng xuất
            return RedirectToAction("Login", "Auth");  // Chuyển hướng về trang đăng nhập
        }
    }


}