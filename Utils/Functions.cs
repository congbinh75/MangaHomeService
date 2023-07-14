using MangaHomeService.Models;
using System.Security.Claims;

namespace MangaHomeService.Utils
{
    public static class Functions
    {
        public static string GetCurrentUserId()
        {
            string currentUser = "";
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                currentUser = claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "";
            }
            else
            {
                throw new Exception();
            }
            return currentUser;
        }
    }
}
