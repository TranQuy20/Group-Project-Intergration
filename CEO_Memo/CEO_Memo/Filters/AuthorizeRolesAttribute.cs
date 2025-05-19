using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CEO_Memo.Helpers;

namespace CEO_Memo.Filters
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedRoles;

        public AuthorizeRolesAttribute(params string[] roles)
        {
            this.allowedRoles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var userRole = httpContext.Session["UserRole"] as string;
            return userRole != null && allowedRoles.Contains(userRole);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Home/AccessDenied");
        }
    }
}
