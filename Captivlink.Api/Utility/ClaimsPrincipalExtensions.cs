using System.Security.Claims;
using System;
using System.Linq;

namespace Captivlink.Api.Utility
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user?.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
        }

        public static Guid GetUserGuid(this ClaimsPrincipal user)
        {
            var userId = user.GetUserId();
            return string.IsNullOrWhiteSpace(userId) ? Guid.Empty : Guid.Parse(userId);
        }

        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user?.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
        }
        public static string GetRole(this ClaimsPrincipal user)
        {
            return user?.Claims.FirstOrDefault(x => x.Type == "role")?.Value;
        }
    }
}
