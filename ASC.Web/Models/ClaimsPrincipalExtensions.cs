using System.Security.Claims;

namespace ASC.Web.Models
{
    public static class ClaimsPrincipalExtensions
    {
        public static CurrentUser? GetCurrentUserDetails(this ClaimsPrincipal? principal)
        {
            if (principal == null || !principal.Identity!.IsAuthenticated)
                return null;

            return new CurrentUser
            {
                Name = principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                Email = principal.FindFirstValue(ClaimTypes.Email)
                    ?? principal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
                    ?? string.Empty,
                IsActive = bool.Parse(principal.FindFirstValue("IsActive") ?? "false")
            };
        }
    }
}
