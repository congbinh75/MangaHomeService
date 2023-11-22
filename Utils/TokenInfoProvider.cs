using System.Security.Claims;

namespace MangaHomeService.Utils
{
    public interface ITokenInfoProvider
    {
        string Id { get; }
        string Name { get; }
        string Role { get; }
    }

    public class TokenInfoProvider : ITokenInfoProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenInfoProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Id => _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        public string Name => _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        public string Role => _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
    }
}
