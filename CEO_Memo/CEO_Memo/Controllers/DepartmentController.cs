using CEO_Memo.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CEO_Memo.Controllers
{
    public class DepartmentController : Controller
    {
        private HumanContext db = new HumanContext();

        // Xem danh sách phòng ban
        public ActionResult Index()
        {
            var list = db.Departments.ToList();
            return View(list);
        }
    }

}