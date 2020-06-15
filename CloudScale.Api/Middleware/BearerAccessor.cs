using System;
using CloudScale.Core;
using Microsoft.AspNetCore.Http;

namespace CloudScale.Api.Middleware
{
    public class BearerAccessor : IBearerAccessor
    {
        private readonly IHttpContextAccessor _accessor;

        public BearerAccessor(IHttpContextAccessor accessor)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        public string Bearer
        {
            get
            {
                string authHeader = _accessor.HttpContext.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Bearer"))
                    return authHeader.Replace("Bearer ", "");

                return null;
            }
        }
    }
}