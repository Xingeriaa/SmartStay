using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace do_an_tot_nghiep.Filters
{
    /// <summary>
    /// Kiểm tra người dùng đã đăng nhập chưa (via Session).
    /// Nếu chưa → redirect về /Account/Login.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userName = context.HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userName))
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl });
            }
            base.OnActionExecuting(context);
        }
    }

    /// <summary>
    /// Kiểm tra role người dùng từ Session.
    /// Chưa đăng nhập → redirect Login.
    /// Sai role → redirect AccessDenied.
    ///
    /// Ví dụ:
    ///   [RequireRole(Roles.Admin)]
    ///   [RequireRole(Roles.AdminOrStaff)]   // "Admin,Staff"
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class RequireRoleAttribute : ActionFilterAttribute
    {
        private readonly HashSet<string> _allowed;

        /// <param name="roles">
        /// Một hoặc nhiều tên role, có thể cách nhau dấu phẩy.
        /// VD: "Admin"  hoặc  "Admin,Staff"
        /// </param>
        public RequireRoleAttribute(params string[] roles)
        {
            _allowed = new HashSet<string>(
                roles.SelectMany(r =>
                    r.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)),
                StringComparer.OrdinalIgnoreCase);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userName = session.GetString("UserName");
            var role = session.GetString("Role") ?? "";

            if (string.IsNullOrEmpty(userName))
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl });
                return;
            }

            if (!_allowed.Contains(role))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
