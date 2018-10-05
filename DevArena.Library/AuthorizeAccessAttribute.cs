using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DevArena.Library
{
    public class AuthorizeAccessAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private RoleEnum _role = RoleEnum.None;

        public AuthorizeAccessAttribute(long? role)
        {
            if (role.HasValue)
                _role = (RoleEnum) role;
        }

        public AuthorizeAccessAttribute(RoleEnum role = RoleEnum.None)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
                return;

            //get scope claims

            //if more then one devarena.api* scope then calculate over user's role
            if (user.Claims != null)
            {
                var scopeClaims = user.Claims.Where(_ => _.Type == "scope" && _.Value.StartsWith("devarena.api"));

                if (scopeClaims.Count() == 0) //should not happen
                    context.Result = new ForbidResult();

                else if (scopeClaims.Any(_ => _.Value== "devarena.api") /*--->swagger*/ 
                         || scopeClaims.Count() > 1 /*---->cannot be sure about access so use role*/ ) //scopes not explicitly defined //could be from swagger //get user's role to se permissions
                {
                    var roleClaim = user.Claims.FirstOrDefault(_ => _.Type == "role");

                    if (roleClaim != null)
                    {
                        var userRole = (RoleEnum) (Convert.ToInt32(roleClaim.Value));
                        if (userRole == RoleEnum.None || (long)userRole > (long)_role)
                            context.Result = new ForbidResult();

                        return;
                    }
                }

                else if (scopeClaims.Count() == 1)
                {
                    var access = scopeClaims.Single().Value.Replace("devarena.api", "");

                    if (access.Contains("limited") && _role == RoleEnum.Administrator)
                        context.Result = new ForbidResult();
                }
            }

        }
    }
}
