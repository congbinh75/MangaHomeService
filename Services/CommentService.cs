using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public interface ICommentService
    {
        public Task<ICollection<Comment>> Get(string id, string type, int pageNumber = 1, int pageSize = Constants.CommentsPerPage);
        public Task<Comment> Add(string id, string type, string content);
        public Task<Comment> Update(string id, string content, int? vote = null);
        public Task<bool> Delete(string id);
        public Task<Comment> AddVote(string id, bool isUpvote);
        public Task<Comment> RemoveVote(string id);
    }

    public class CommentService : ICommentService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public CommentService(IDbContextFactory<MangaHomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<ICollection<Comment>> Get(string id, string type, int pageNumber = 1, int pageSize = Constants.CommentsPerPage)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            if (type == typeof(Title).Name)
            {
                var title = await dbContext.Titles.Where(t => t.Id == id).
                    Include(t => t.Comments.Skip(pageSize * (pageNumber - 1)).Take(pageSize)).FirstOrDefaultAsync() ??
                    throw new NotFoundException(typeof(Title).Name);
                return title.Comments;
            }
            else if (type == typeof(Chapter).Name)
            {
                var chapter = await dbContext.Chapters.Where(c => c.Id == id).
                    Include(t => t.Comments.Skip(pageSize * (pageNumber - 1)).Take(pageSize)).FirstOrDefaultAsync() ??
                    throw new NotFoundException(typeof(Chapter).Name);
                return chapter.Comments;
            }
            else if (type == typeof(Group).Name)
            {
                var group = await dbContext.Groups.Where(g => g.Id == id).
                    Include(t => t.Comments.Skip(pageSize * (pageNumber - 1)).Take(pageSize)).FirstOrDefaultAsync() ??
                    throw new NotFoundException(typeof(Group).Name);
                return group.Comments;
            }
            else
            {
                throw new ArgumentException(type);
            }
        }

        public async Task<Comment> Add(string id, string type, string content)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            if (type == typeof(Title).Name)
            {
                var title = await dbContext.Titles.Where(t => t.Id == id).FirstOrDefaultAsync() ??
                    throw new NotFoundException(typeof(Title).Name);
                var comment = new TitleComment
                {
                    Title = title,
                    Content = content,
                    Vote = 0
                };
                await dbContext.Comments.AddAsync(comment);
                await dbContext.SaveChangesAsync();
                return comment;
            }
            else if (type == typeof(Chapter).Name)
            {
                var chapter = await dbContext.Chapters.Where(c => c.Id == id).FirstOrDefaultAsync() ??
                    throw new NotFoundException(typeof(Chapter).Name);
                var comment = new ChapterComment
                {
                    Chapter = chapter,
                    Content = content,
                    Vote = 0
                };
                await dbContext.Comments.AddAsync(comment);
                await dbContext.SaveChangesAsync();
                return comment;
            }
            else if (type == typeof(Group).Name)
            {
                var group = await dbContext.Groups.Where(g => g.Id == id).FirstOrDefaultAsync() ??
                    throw new NotFoundException(typeof(Group).Name);
                var comment = new GroupComment
                {
                    Group = group,
                    Content = content,
                    Vote = 0
                };
                await dbContext.Comments.AddAsync(comment);
                await dbContext.SaveChangesAsync();
                return comment;
            }
            else
            {
                throw new ArgumentException(type);
            }
        }

        public async Task<Comment> Update(string id, string content, int? vote = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var comment = await dbContext.Comments.Where(c => c.Id == id).FirstOrDefaultAsync() ??
                throw new NotFoundException(typeof(Comment).Name);
            comment.Content = content;
            comment.Vote = vote == null ? comment.Vote : (int)vote;
            await dbContext.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> Delete(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var comment = await dbContext.Comments.Where(c => c.Id == id).FirstOrDefaultAsync() ??
                throw new NotFoundException(typeof(Comment).Name);
            dbContext.Comments.Remove(comment);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Comment> AddVote(string id, bool isUpvote)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id) ??
                throw new NotFoundException(typeof(Comment).Name);
            var vote = new CommentVote
            {
                Comment = comment,
                IsUpvote = isUpvote
            };
            await dbContext.CommentVotes.AddAsync(vote);
            await dbContext.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> RemoveVote(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id) ??
                throw new NotFoundException(typeof(Comment).Name);
            var vote = await dbContext.CommentVotes.Where(v => v.Comment.Id == id && v.User.Id == v.User.Id).FirstOrDefaultAsync() ??
                throw new NotFoundException(typeof(CommentVote).Name);
            dbContext.CommentVotes.Remove(vote);
            await dbContext.SaveChangesAsync();
            return comment;
        }
    }
}
