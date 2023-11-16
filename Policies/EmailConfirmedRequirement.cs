using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Policies
{
    public class EmailConfirmedRequirement : IAuthorizationRequirement { }
    public class EmailConfirmedRequirementHandler : AuthorizationHandler<EmailConfirmedRequirement>
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;
        public EmailConfirmedRequirementHandler(IDbContextFactory<MangaHomeDbContext> contextFactory) 
        {
            _contextFactory = contextFactory;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EmailConfirmedRequirement requirement)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == Functions.GetCurrentUserId());
                if (user != null && !user.IsEmailConfirmed)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
