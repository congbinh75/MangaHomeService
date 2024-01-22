using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Policies
{
    public class EmailConfirmedRequirement : IAuthorizationRequirement { }
    
    public class EmailConfirmedRequirementHandler(IDbContextFactory<MangaHomeDbContext> contextFactory, ITokenInfoProvider tokenInfoProvider) : AuthorizationHandler<EmailConfirmedRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EmailConfirmedRequirement requirement)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == tokenInfoProvider.Id);
            if (user != null && !user.IsEmailConfirmed)
            {
                context.Succeed(requirement);
            }
        }
    }
}
