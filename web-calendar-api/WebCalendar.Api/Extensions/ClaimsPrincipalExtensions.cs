using System.Security.Claims;

namespace WebCalendar.Api.Extensions
{
  internal static class ClaimsPrincipalExtensions
  {
    public static int GetId(this ClaimsPrincipal claimsPrincipal)
    {
      var id = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value;
      return int.Parse(id);
    }
  }
}
