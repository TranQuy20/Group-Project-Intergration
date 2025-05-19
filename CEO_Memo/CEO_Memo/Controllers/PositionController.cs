using CEO_Memo.DAL;
using CEO_Memo.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CEO_Memo.Controllers
{
    [AuthorizeRoles("Admin", "HR")]
    public class PositionController : Controller
    {
        private HumanContext db = new HumanContext();

        // Xem danh sách chức vụ
        public ActionResult Index()
        {
            var list = db.Positions.ToList();
            return View(list);
        }
    }

}