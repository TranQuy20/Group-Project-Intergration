using CEO_Memo.DAL;
using CEO_Memo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static System.Collections.Specialized.BitVector32;
using System.Web.Mvc;

namespace CEO_Memo.Controllers
{
    public class AuthController : Controller
    {
        private HumanContext db = new HumanContext();  // SQL Server

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Employees.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);
                if (user != null)
                {
                    Session["UserID"] = user.EmployeeID;
                    Session["UserName"] = user.FullName;
                    Session["UserRole"] = user.Role; // "Admin", "HR", "Payroll", "Staff"

                    return RedirectToAction("Dashboard", "Home");
                }
                ViewBag.Message = "Sai tên đăng nhập hoặc mật khẩu!";
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }

}