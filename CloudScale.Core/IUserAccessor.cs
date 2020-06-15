using System.Security.Claims;

namespace CloudScale.Core
{
    public interface IUserAccessor
    {
        ClaimsPrincipal User { get; }
    }
}