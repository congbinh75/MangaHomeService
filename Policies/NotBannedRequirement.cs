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

        public NotBannedRequirementHandler(IDbContextFactory<MangaHomeDbContext> contextFactory) 
        {
            _contextFactory = contextFactory;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotBannedRequirement requirement)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync()) 
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == Functions.GetCurrentUserId());
                if (user != null && !user.IsBanned)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
