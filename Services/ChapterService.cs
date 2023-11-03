using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public class ChapterService : IChapterService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public ChapterService(IDbContextFactory<MangaHomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Chapter> Add(double number, string titleId, string groupId, string? volumeId = null, string? languageId = null,
            List<string>? pagesIds = null, List<string>? commentsIds = null, bool isApproved = false)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == titleId);
                if (title == null)
                {
                    throw new ArgumentException(nameof(title));
                }
                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
                if (group == null)
                {
                    throw new ArgumentException(nameof(group));
                }
                var volume = volumeId == null ? null : await dbContext.Volumes.FirstOrDefaultAsync(v => v.Id == volumeId);
                if (volume == null)
                {
                    throw new ArgumentException(nameof(volume));
                }
                var language = languageId == null ? null : await dbContext.Languages.FirstOrDefaultAsync(l => l.Id == languageId);
                if (language == null)
                {
                    throw new ArgumentException(nameof(language));
                }
                var pages = pagesIds == null ? null : await dbContext.Pages.Where(p => pagesIds.Contains(p.Id)).ToListAsync();
                var comments = commentsIds == null ? null : await dbContext.Comments.Where(c => commentsIds.Contains(c.Id)).ToListAsync();

                Chapter chapter = new Chapter();
                chapter.Number = number;
                chapter.Title = title;
                chapter.Group = group;
                chapter.Volume = volume;
                chapter.Language = language;
                chapter.Pages = pages;
                chapter.Comments = comments;
                chapter.IsApproved = isApproved;

                await dbContext.Chapters.AddAsync(chapter);
                await dbContext.SaveChangesAsync();
                return chapter;
            }
        }

        public async Task<bool> Delete(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var chapter = await dbContext.Chapters.Where(c => c.Id == id).Include(c => c.Pages).Include(c => c.Comments).
                    FirstOrDefaultAsync();
                if (chapter == null)
                {
                    throw new ArgumentException(nameof(chapter));
                }
                dbContext.Chapters.Remove(chapter);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<Chapter> Get(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var chapter = await dbContext.Chapters.Where(c => c.Id == id).Include(c => c.Pages).FirstOrDefaultAsync();
                if (chapter == null)
                {
                    throw new ArgumentException(nameof(chapter));
                }
                return chapter;
            }
        }

        public async Task<List<Chapter>> GetByTitle(string titleId)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == titleId);
                if (title == null)
                {
                    throw new ArgumentException(nameof(title));
                }
                return await dbContext.Chapters.Where(c => c.Title.Id == titleId).OrderByDescending(c => c.Number).ToListAsync();
            }
        }

        public async Task<Chapter> Update(string id, double number = 0, string? titleId = null, string? groupId = null, string? volumeId = null,
            string? languageId = null, List<string>? pagesIds = null, List<string>? commentsIds = null, bool? isApproved = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var chapter = await dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == id);
                if (chapter == null)
                {
                    throw new ArgumentException(nameof(chapter));
                }

                var newNumber = number > 0 ? number : chapter.Number;
                var newTitle = titleId != null ? await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == titleId) : chapter.Title;
                var newGroup = groupId != null ? await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId) : chapter.Group;
                var newVolume = volumeId != null ? await dbContext.Volumes.FirstOrDefaultAsync(v => v.Id == volumeId) : chapter.Volume;
                var newLanguage = languageId != null ? await dbContext.Languages.FirstOrDefaultAsync(l => l.Id == languageId) : chapter.Language;
                var newPages = pagesIds != null ? await dbContext.Pages.Where(p => pagesIds.Contains(p.Id)).ToListAsync() : chapter.Pages;
                var newComments = commentsIds != null ? await dbContext.Comments.Where(c => commentsIds.Contains(c.Id)).ToListAsync() : chapter.Comments;
                var newIsApproved = isApproved != null ? isApproved : chapter.IsApproved;

                chapter.Number = newNumber;
                chapter.Title = newTitle != null ? newTitle : chapter.Title;
                chapter.Group = newGroup != null ? newGroup : chapter.Group;
                chapter.Volume = newVolume != null ? newVolume : chapter.Volume;
                chapter.Language = newLanguage != null ? newLanguage : chapter.Language;
                chapter.Pages = newPages;
                chapter.Comments = newComments;
                chapter.IsApproved = (bool)newIsApproved;

                await dbContext.SaveChangesAsync();
                return chapter;
            }
        }

        public async Task<ChapterRequest> SubmitRequest(string titleId, string groupId)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var chapter = await dbContext.Chapters.FirstOrDefaultAsync(t => t.Id == titleId);
                if (chapter == null)
                {
                    throw new ArgumentException(nameof(chapter));
                }
                var submitUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == Functions.GetCurrentUserId());
                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
                if (group == null)
                {
                    throw new ArgumentException(nameof(group));
                }

                var request = new ChapterRequest();
                request.Chapter = chapter;
                request.SubmitUser = submitUser;
                request.Group = group;
                request.IsApproved = false;
                request.IsReviewed = false;

                await dbContext.ChapterRequests.AddAsync(request);
                await dbContext.SaveChangesAsync();
                return request;
            }
        }

        public async Task<ChapterRequest> GetRequest(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var request = await dbContext.ChapterRequests.Where(r => r.Id == id).Include(r => r.Chapter).FirstOrDefaultAsync();
                if (request == null)
                {
                    throw new ArgumentException(nameof(request));
                }
                if (request.Chapter == null)
                {
                    throw new ArgumentException(nameof(request.Chapter));
                }
                return request;
            }
        }

        public async Task<ChapterRequest> ReviewRequest(string requestId, bool isApproved)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var reviewUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == Functions.GetCurrentUserId());
                var request = await dbContext.ChapterRequests.FirstOrDefaultAsync(r => r.Id == requestId);
                if (request == null)
                {
                    throw new ArgumentException(nameof(request));
                }
                if (request.IsReviewed)
                {
                    throw new ArgumentException();
                }

                request.IsApproved = isApproved;
                request.Chapter.IsApproved = isApproved;
                request.IsReviewed = true;
                request.ReviewUser = reviewUser;

                await dbContext.SaveChangesAsync();
                return request;
            }
        }

        public Task<List<Comment>> GetComments(string id, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<Comment> AddComment(string chapterId)
        {
            throw new NotImplementedException();
        }

        public Task<Comment> UpdateComment(string commentId)
        {
            throw new NotImplementedException();
        }

        public Task<Comment> DeleteComment(string commentId)
        {
            throw new NotImplementedException();
        }
    }
}
