using System;
using System.Security.Claims;
using CloudScale.Core;
using Microsoft.AspNetCore.Http;

namespace CloudScale.Api.Middleware
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _accessor;

        public UserAccessor(IHttpContextAccessor accessor)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        public ClaimsPrincipal User => _accessor.HttpContext.User;
    }
}