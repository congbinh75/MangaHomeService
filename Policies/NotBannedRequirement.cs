using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Policies
{
    public class NotBannedRequirement : IAuthorizationRequirement { }

    public class NotBannedRequirementHandler(IDbContextFactory<MangaHomeDbContext> contextFactory, ITokenInfoProvider tokenInfoProvider) : AuthorizationHandler<NotBannedRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotBannedRequirement requirement)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == tokenInfoProvider.Id);
            if (user != null && !user.IsBanned)
            {
                context.Succeed(requirement);
            }
        }
    }
}
