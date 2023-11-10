using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace MangaHomeService.Services
{
    public class GroupService : IGroupService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public GroupService(IDbContextFactory<MangaHomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public Task<Group> Add(string name, string description, string profilePicture, List<string> membersIds)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Group> Get(string id)
        {
            throw new NotImplementedException();
        }

        public Task<TitleRequest> ReviewRequest(string requestId, bool isApproved)
        {
            throw new NotImplementedException();
        }

        public Task<TitleRequest> SubmitRequest(string groupId)
        {
            throw new NotImplementedException();
        }

        public Task<Group> Update(string id, string? name = null, string? description = null, string? profilePicture = null, 
            List<string>? membersIds = null)
        {
            throw new NotImplementedException();
        }
        public async Task<List<Comment>> GetComments(string id, int pageNumber = 1, int pageSize = Constants.CommentsPerPage)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var comments = await dbContext.Comments.Where(c => c.Title.Id == id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                return comments;
            }
        }

        public async Task<Comment> AddComment(string groupId, string content)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var commnentUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == Functions.GetCurrentUserId());
                if (!commnentUser.EmailConfirmed)
                {
                    throw new EmailNotConfirmedException();
                }

                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
                if (group == null)
                {
                    throw new ArgumentException(nameof(groupId));
                }

                if (string.IsNullOrEmpty(content))
                {
                    throw new ArgumentException(nameof(content));
                }

                var comment = new Comment();
                comment.Group = group;
                comment.Content = content;
                await dbContext.Comments.AddAsync(comment);
                return comment;
            }
        }

        public async Task<Comment> UpdateComment(string commentId, string? content = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                if (content == "")
                {
                    throw new ArgumentException(nameof(content));
                }

                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
                if (comment == null)
                {
                    throw new ArgumentException(nameof(commentId));
                }

                comment.Content = content != null ? content : comment.Content;
                await dbContext.Comments.AddAsync(comment);
                return comment;
            }
        }

        public async Task<bool> DeleteComment(string commentId)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
                if (comment == null)
                {
                    throw new ArgumentException(nameof(commentId));
                }

                dbContext.Comments.Remove(comment);
                return true;
            }
        }
    }
}
