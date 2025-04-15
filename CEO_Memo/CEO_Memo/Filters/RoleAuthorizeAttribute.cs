using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CEO_Memo.Filters
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        public string Role { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var role = (string)httpContext.Session["UserRole"];
            return role != null && role == Role;
        }
    }

}