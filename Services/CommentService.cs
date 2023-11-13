using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public class CommentService : ICommentService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public CommentService(IDbContextFactory<MangaHomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Comment>> Get(string id, string type, int pageNumber = 1, int pageSize = Constants.CommentsPerPage)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                if (type == typeof(Title).Name)
                {
                    var title = await dbContext.Titles.Where(t => t.Id == id).Include(t => t.Comments).FirstOrDefaultAsync();
                    if (title == null)
                    {
                        throw new NotFoundException(typeof(Title).Name);
                    }
                    return title.Comments;
                }
                else if (type == typeof(Chapter).Name)
                {
                    var chapter = await dbContext.Chapters.Where(c => c.Id == id).Include(c => c.Comments).FirstOrDefaultAsync();
                    if (chapter == null)
                    {
                        throw new NotFoundException(typeof(Chapter).Name);
                    }
                    return chapter.Comments;
                }
                else if (type == typeof(Group).Name)
                {
                    var group = await dbContext.Groups.Where(g => g.Id == id).Include(g => g.Comments).FirstOrDefaultAsync();
                    if (group == null)
                    {
                        throw new NotFoundException(typeof(Group).Name);
                    }
                    return group.Comments;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        public async Task<Comment> Add(string id, string type, string content)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                if (type == typeof(Title).Name)
                {
                    var title = await dbContext.Titles.Where(t => t.Id == id).FirstOrDefaultAsync();
                    if (title == null)
                    {
                        throw new NotFoundException(typeof(Title).Name);
                    }

                    var comment = new Comment();
                    comment.Title = title;
                    comment.Content = content;
                    comment.Vote = 0;
                    await dbContext.Comments.AddAsync(comment);
                    await dbContext.SaveChangesAsync();
                    return comment;
                }
                else if (type == typeof(Chapter).Name)
                {
                    var chapter = await dbContext.Chapters.Where(c => c.Id == id).FirstOrDefaultAsync();
                    if (chapter == null)
                    {
                        throw new NotFoundException(typeof(Chapter).Name);
                    }

                    var comment = new Comment();
                    comment.Chapter = chapter;
                    comment.Content = content;
                    comment.Vote = 0;
                    await dbContext.Comments.AddAsync(comment);
                    await dbContext.SaveChangesAsync();
                    return comment;
                }
                else if (type == typeof(Group).Name)
                {
                    var group = await dbContext.Groups.Where(g => g.Id == id).FirstOrDefaultAsync();
                    if (group == null)
                    {
                        throw new NotFoundException(typeof(Group).Name);
                    }

                    var comment = new Comment();
                    comment.Group = group;
                    comment.Content = content;
                    comment.Vote = 0;
                    await dbContext.Comments.AddAsync(comment);
                    await dbContext.SaveChangesAsync();
                    return comment;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        public async Task<Comment> Update(string id, string content, int? vote = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var comment = await dbContext.Comments.Where(c => c.Id == id).FirstOrDefaultAsync();
                if (comment == null)
                {
                    throw new NotFoundException(typeof(Comment).Name);
                }

                comment.Content = content;
                comment.Vote = vote == null ? comment.Vote : (int)vote;
                await dbContext.SaveChangesAsync();
                return comment;
            }
        }

        public async Task<bool> Delete(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var comment = await dbContext.Comments.Where(c => c.Id == id).FirstOrDefaultAsync();
                if (comment == null)
                {
                    throw new NotFoundException(typeof(Comment).Name);
                }

                dbContext.Comments.Remove(comment);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<Comment> Vote(string id, bool isUpvote)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);
                if (comment == null)
                {
                    throw new NotFoundException(typeof(Comment).Name);
                }

                var votingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == Functions.GetCurrentUserId());
                if (!votingUser.EmailConfirmed)
                {
                    throw new EmailNotConfirmedException();
                }

                var vote = new CommentVote();
                vote.Comment = comment;
                vote.IsUpvote = isUpvote;
                await dbContext.CommentVotes.AddAsync(vote);
                await dbContext.SaveChangesAsync();
                return comment;
            }
        }

        public async Task<Comment> RemoveVote(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);
                if (comment == null)
                {
                    throw new NotFoundException(typeof(Comment).Name);
                }

                var votingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == Functions.GetCurrentUserId());
                if (!votingUser.EmailConfirmed)
                {
                    throw new EmailNotConfirmedException();
                }

                var vote = await dbContext.CommentVotes.Where(v => v.Comment.Id == id && v.User.Id == votingUser.Id).FirstOrDefaultAsync();
                if (vote == null) 
                {
                    throw new NotFoundException(typeof(CommentVote).Name);
                }

                dbContext.CommentVotes.Remove(vote);
                await dbContext.SaveChangesAsync();
                return comment;
            }
        }
    }
}
