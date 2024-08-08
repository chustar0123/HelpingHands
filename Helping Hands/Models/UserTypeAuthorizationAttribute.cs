using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Helping_Hands.Models
{
    public class UserTypeAuthorizationAttribute : TypeFilterAttribute
    {
        public UserTypeAuthorizationAttribute(params string[] allowedUserTypes) : base(typeof(UserTypeAuthorizationFilter))
        {
            Arguments = new object[] { allowedUserTypes };
        }
        public class UserTypeAuthorizationFilter : IAuthorizationFilter
        {
            private readonly string[] _allowedUserTypes;

            public UserTypeAuthorizationFilter(string[] allowedUserTypes)
            {
                _allowedUserTypes = allowedUserTypes;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var userType = context.HttpContext.Session.GetString("UserType");
                if (!_allowedUserTypes.Contains(userType))
                {
                    // Set an error message in TempData
                    context.HttpContext.Session.SetString("ErrorMessage", "You are not authorized to access this page.");
                    context.Result = new RedirectToActionResult("ErrorPage", "Home", null); // Redirect to an error page
                }
            }
        }
    }
}
