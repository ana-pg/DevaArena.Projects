using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DevArena.Library
{
    public class AuthorizeByRoleAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private RoleEnum _role = RoleEnum.None;

        public AuthorizeByRoleAttribute(long? role)
        {
            if (role.HasValue)
                _role = (RoleEnum) role;
        }

        public AuthorizeByRoleAttribute(RoleEnum role = RoleEnum.None)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
                return;

            var currentUserRole = RoleEnum.None;
            if (user.Claims != null)
            {
                var trusted = user.Claims.Any(_ => _.Type == "scope" && _.Value == "devarena.api.client_access");
                if (trusted)
                    return;

                var roleClaim = user.Claims.FirstOrDefault(_ => _.Type == "role");
                if (roleClaim != null)
                    currentUserRole = (RoleEnum) (Convert.ToInt32(roleClaim.Value));
            }

            if (currentUserRole == RoleEnum.None || (long) currentUserRole > (long) _role)
                context.Result = new ForbidResult();

        }
    }
}
