using MangaHomeService.Models;
using MangaHomeService.Models.Entities;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public interface ICommentService
    {
        public Task<ICollection<Comment>> Get(string id, string type, int pageNumber = 1, int pageSize = Constants.CommentsPerPage);
        public Task<Comment> Add(string id, Type type, string content);
        public Task<Comment> Update(string id, string? content = null, int? vote = null);
        public Task<bool> Remove(string id);
        public Task<Comment> AddVote(string id, bool isUpvote, string? userId = null);
        public Task<Comment> RemoveVote(string id, string? userId = null);
    }

    public class CommentService : ICommentService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;
        private readonly ITokenInfoProvider _tokenInfoProvider;

        public CommentService(IDbContextFactory<MangaHomeDbContext> contextFactory, ITokenInfoProvider tokenInfoProvider)
        {
            _contextFactory = contextFactory;
            _tokenInfoProvider = tokenInfoProvider;
        }

        public async Task<ICollection<Comment>> Get(string id, string type, int pageNumber = 1, int pageSize = Constants.CommentsPerPage)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            if (type == nameof(Title))
            {
                var title = await dbContext.Titles.Where(t => t.Id == id).
                    Include(t => t.Comments.Skip(pageSize * (pageNumber - 1)).Take(pageSize)).FirstOrDefaultAsync() ??
                    throw new NotFoundException(nameof(Title));
                return title.Comments;
            }
            else if (type == nameof(Chapter))
            {
                var chapter = await dbContext.Chapters.Where(c => c.Id == id).
                    Include(t => t.Comments.Skip(pageSize * (pageNumber - 1)).Take(pageSize)).FirstOrDefaultAsync() ??
                    throw new NotFoundException(nameof(Chapter));
                return chapter.Comments;
            }
            else if (type == nameof(Group))
            {
                var group = await dbContext.Groups.Where(g => g.Id == id).
                    Include(t => t.Comments.Skip(pageSize * (pageNumber - 1)).Take(pageSize)).FirstOrDefaultAsync() ??
                    throw new NotFoundException(nameof(Group));
                return group.Comments;
            }
            else
            {
                throw new ArgumentException(type);
            }
        }

        public async Task<Comment> Add(string id, Type type, string content)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            if (type == typeof(Title))
            {
                var title = await dbContext.Titles.Where(t => t.Id == id).FirstOrDefaultAsync() ??
                    throw new NotFoundException(nameof(Title));
                var comment = new TitleComment
                {
                    Title = title,
                    Content = content,
                    Vote = 0,
                    CommentVotes = new List<CommentVote>()
                };
                await dbContext.Comments.AddAsync(comment);
                await dbContext.SaveChangesAsync();
                return comment;
            }
            else if (type == typeof(Chapter))
            {
                var chapter = await dbContext.Chapters.Where(c => c.Id == id).FirstOrDefaultAsync() ??
                    throw new NotFoundException(nameof(Chapter));
                var comment = new ChapterComment
                {
                    Chapter = chapter,
                    Content = content,
                    Vote = 0,
                    CommentVotes = new List<CommentVote>()
                };
                await dbContext.Comments.AddAsync(comment);
                await dbContext.SaveChangesAsync();
                return comment;
            }
            else if (type == typeof(Group))
            {
                var group = await dbContext.Groups.Where(g => g.Id == id).FirstOrDefaultAsync() ??
                    throw new NotFoundException(nameof(Group));
                var comment = new GroupComment
                {
                    Group = group,
                    Content = content,
                    Vote = 0,
                    CommentVotes = new List<CommentVote>()
                };
                await dbContext.Comments.AddAsync(comment);
                await dbContext.SaveChangesAsync();
                return comment;
            }
            else
            {
                throw new ArgumentException(nameof(type));
            }
        }

        public async Task<Comment> Update(string id, string? content = null, int? vote = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var comment = await dbContext.Comments.Where(c => c.Id == id).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(Comment));
            comment.Content = content ?? comment.Content;
            comment.Vote = vote == null ? comment.Vote : (int)vote;
            await dbContext.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> Remove(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var comment = await dbContext.Comments.Where(c => c.Id == id).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(Comment));
            dbContext.Comments.Remove(comment);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Comment> AddVote(string id, bool isUpvote, string? userId = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id) ??
                throw new NotFoundException(nameof(Comment));
            var votingUserId = userId ?? _tokenInfoProvider.Id;
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == votingUserId) ?? throw new NotFoundException(nameof(User));
            var vote = new CommentVote
            {
                Comment = comment,
                IsUpvote = isUpvote,
                User = user,
                CommentId = comment.Id,
                UserId = user.Id
            };
            await dbContext.CommentVotes.AddAsync(vote);
            await dbContext.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> RemoveVote(string id, string? userId = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id) ??
                throw new NotFoundException(nameof(Comment));
            var votingUserId = userId ?? _tokenInfoProvider.Id;
            var vote = await dbContext.CommentVotes.Where(v => v.Comment.Id == id && v.User.Id == votingUserId).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(CommentVote));
            dbContext.CommentVotes.Remove(vote);
            await dbContext.SaveChangesAsync();
            return comment;
        }
    }
}
