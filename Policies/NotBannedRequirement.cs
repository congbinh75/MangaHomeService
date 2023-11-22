using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Policies
{
    public class NotBannedRequirement : IAuthorizationRequirement { }

    public class NotBannedRequirementHandler : AuthorizationHandler<NotBannedRequirement>
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;
        private readonly ITokenInfoProvider _tokenInfoProvider;

        public NotBannedRequirementHandler(IDbContextFactory<MangaHomeDbContext> contextFactory, ITokenInfoProvider tokenInfoProvider)
        {
            _contextFactory = contextFactory;
            _tokenInfoProvider = tokenInfoProvider;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotBannedRequirement requirement)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == _tokenInfoProvider.Id);
                if (user != null && !user.IsBanned)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
