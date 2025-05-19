using CEO_Memo.DAL;
using CEO_Memo.Filters;
using CEO_Memo.Models;  // Đảm bảo bạn thêm namespace chứa lớp Department
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CEO_Memo.Controllers
{
    [AuthorizeRoles("Admin", "HR")]
    public class DepartmentController : Controller
    {
        private HumanContext db = new HumanContext();

        public ActionResult Index()
        {
            var list = db.Departments.ToList();
            return View(list);
        }
    }
}
