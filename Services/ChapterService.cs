using MangaHomeService.Models;
using MangaHomeService.Models.Entities;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public class ChapterService(IDbContextFactory<MangaHomeDbContext> contextFactory, ITokenInfoProvider tokenInfoProvider) : IChapterService
    {
        public async Task<Chapter> Get(string id)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var chapter = await dbContext.Chapters.Where(c => c.Id == id).Include(c => c.Pages).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(Chapter));
            return chapter;
        }

        public async Task<ICollection<Chapter>> GetByTitle(string titleId, int pageNumber = 1, int pageSize = Constants.ChaptersPerPage)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == titleId) ??
                throw new NotFoundException(nameof(Title));
            return await dbContext.Chapters.Where(c => c.Title.Id == titleId && c.IsApproved).OrderByDescending(c => c.Number).
                GroupBy(c => c.Number).SelectMany(c => c).Distinct().Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
        }

        public async Task<Chapter> Add(double number, string titleId, string groupId, string? volumeId = null, string? languageId = null,
            ICollection<string>? pagesIds = null, ICollection<string>? commentsIds = null, bool isApproved = false, bool isRemoved = false)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();

            var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == titleId) ?? throw new NotFoundException(nameof(Title));
            title.CheckUploadConditions();

            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId) ?? throw new NotFoundException(nameof(Group));
            group.CheckUploadContions();

            var volume = (volumeId == null ? null : await dbContext.Volumes.FirstOrDefaultAsync(v => v.Id == volumeId)) ??
                throw new NotFoundException(nameof(Volume));
            var language = (languageId == null ? null : await dbContext.Languages.FirstOrDefaultAsync(l => l.Id == languageId)) ??
                throw new NotFoundException(nameof(Language));

            var pages = new List<Page>();
            if (pagesIds != null)
            {
                foreach (var pageId in pagesIds)
                {
                    var page = await dbContext.Pages.FirstOrDefaultAsync(p => p.Id == pageId) ??
                        throw new NotFoundException(nameof(Page));
                    pages.Add(page);
                }
            }

            var comments = new List<Comment>();
            if (commentsIds != null)
            {
                foreach (var commentId in commentsIds)
                {
                    var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId) ??
                        throw new NotFoundException(nameof(Comment));
                    comments.Add(comment);
                }
            }

            var chapter = new Chapter
            {
                Number = number,
                Title = title,
                Group = group,
                Volume = volume,
                Language = language,
                Pages = pages,
                Comments = comments,
                IsApproved = isApproved,
                IsRemoved = isRemoved
            };

            await dbContext.Chapters.AddAsync(chapter);
            await dbContext.SaveChangesAsync();
            return chapter;
        }

        public async Task<Chapter> Update(string id, double? number = null, string? titleId = null, string? groupId = null,
            string? volumeId = null, string? languageId = null, ICollection<string>? pagesIds = null, ICollection<string>? commentsIds = null,
            bool? isApproved = null, bool? isRemoved = false)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var chapter = await dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == id) ??
                throw new NotFoundException(nameof(Chapter));

            var pages = new List<Page>();
            if (pagesIds != null)
            {
                foreach (var pageId in pagesIds)
                {
                    var page = await dbContext.Pages.FirstOrDefaultAsync(p => p.Id == pageId) ??
                        throw new NotFoundException(nameof(Page));
                    pages.Add(page);
                }
            }

            var comments = new List<Comment>();
            if (commentsIds != null)
            {
                foreach (var commentId in commentsIds)
                {
                    var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId) ??
                        throw new NotFoundException(nameof(Comment));
                    comments.Add(comment);
                }
            }

            if (titleId != null)
            {
                var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == titleId) ??
                    throw new NotFoundException(nameof(Title));
                title.CheckUploadConditions();
                chapter.Title = title;
            }

            if (groupId != null)
            {
                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId) ??
                    throw new NotFoundException(nameof(Group));
                group.CheckUploadContions();
                chapter.Group = group;
            }

            chapter.Number = number == null ? chapter.Number : (double)number;
            chapter.Volume = volumeId != null ?
                await dbContext.Volumes.FirstOrDefaultAsync(v => v.Id == volumeId) ??
                throw new NotFoundException(nameof(Volume)) :
                chapter.Volume;
            chapter.Language = languageId != null ?
                await dbContext.Languages.FirstOrDefaultAsync(l => l.Id == languageId) ??
                throw new NotFoundException(nameof(Language)) :
                chapter.Language;
            chapter.Pages = pagesIds == null ? chapter.Pages : pages;
            chapter.Comments = commentsIds == null ? chapter.Comments : comments;
            chapter.IsApproved = isApproved == null ? chapter.IsApproved : (bool)isApproved;
            chapter.IsRemoved = isRemoved == null ? chapter.IsRemoved : (bool)isRemoved;

            await dbContext.SaveChangesAsync();
            return chapter;
        }

        public async Task<bool> Remove(string id)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var chapter = await dbContext.Chapters.FirstOrDefaultAsync() 
                ?? throw new NotFoundException(nameof(Chapter));
            if (chapter.IsRemoved)
            {
                throw new AlreadyRemovedException();
            }
            chapter.IsRemoved = true;
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ChapterRequest> GetRequest(string requestId)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.OfType<ChapterRequest>().Where(r => r.Id == requestId)
                .Include(r => r.Chapter).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(ChapterRequest));
            return request;
        }

        public async Task<ChapterRequest> ReviewRequest(string requestId, string note, bool isApproved)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.OfType<ChapterRequest>().FirstOrDefaultAsync(r => r.Id == requestId) ??
                throw new NotFoundException(nameof(ChapterRequest));
            if (request.IsReviewed)
            {
                throw new AlreadyReviewedException();
            }

            request.ReviewNote = note;
            request.IsApproved = isApproved;
            request.Chapter.IsApproved = isApproved;
            request.IsReviewed = true;

            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<bool> AddTracking(string id, string? userId = null)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var trackingUserId = userId ?? tokenInfoProvider.Id;
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == trackingUserId) ?? throw new NotFoundException(nameof(User));
            var chapter = await dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == id) ?? throw new NotFoundException(nameof(Chapter));
            user.ChapterTrackings.Add(chapter);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveTracking(string id, string? userId = null)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var trackingUserId = userId ?? tokenInfoProvider.Id;
            var user = await dbContext.Users.Where(u => u.Id == trackingUserId).Include(u => u.ChapterTrackings).FirstOrDefaultAsync()
                ?? throw new NotFoundException(nameof(User));
            var tracking = user.ChapterTrackings.FirstOrDefault(c => c.Id == id) ?? throw new NotFoundException(nameof(Chapter));
            user.ChapterTrackings.Remove(tracking);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
