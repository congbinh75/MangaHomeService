using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public interface IChapterService
    {
        public Task<Chapter> Get(string id);
        public Task<ICollection<Chapter>> GetByTitle(string titleId, int pageNumber = 1, int pageSize = Constants.ChaptersPerPage);
        public Task<Chapter> Add(double number, string titleId, string groupId, string? volumeId = null, string? languageId = null,
            ICollection<string>? pagesIds = null, ICollection<string>? commentsIds = null, bool isApproved = false);
        public Task<Chapter> Update(string id, double? number = null, string? titleId = null, string? groupId = null,
            string? volumeId = null, string? languageId = null, ICollection<string>? pagesIds = null, ICollection<string>? commentsIds = null,
            bool? isApproved = null);
        public Task<bool> Delete(string id);
        public Task<ChapterRequest> GetRequest(string requestId);
        public Task<ChapterRequest> SubmitRequest(string chapterId, string note, string groupId);
        public Task<ChapterRequest> ReviewRequest(string requestId, string note, bool isApproved);
    }

    public class ChapterService : IChapterService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public ChapterService(IDbContextFactory<MangaHomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Chapter> Get(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var chapter = await dbContext.Chapters.Where(c => c.Id == id).Include(c => c.Pages).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(Chapter));
            return chapter;
        }

        public async Task<ICollection<Chapter>> GetByTitle(string titleId, int pageNumber = 1, int pageSize = Constants.ChaptersPerPage)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == titleId) ??
                throw new NotFoundException(nameof(Title));
            //TO BE FIXED
            return await dbContext.Chapters.Where(c => c.Title.Id == titleId && c.IsApproved).OrderByDescending(c => c.Number).
                GroupBy(c => c.Number).Select(c => c.First()).Distinct().Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
        }

        public async Task<Chapter> Add(double number, string titleId, string groupId, string? volumeId = null, string? languageId = null,
            ICollection<string>? pagesIds = null, ICollection<string>? commentsIds = null, bool isApproved = false)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();

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
                IsApproved = isApproved
            };

            await dbContext.Chapters.AddAsync(chapter);
            await dbContext.SaveChangesAsync();
            return chapter;
        }

        public async Task<Chapter> Update(string id, double? number = null, string? titleId = null, string? groupId = null,
            string? volumeId = null, string? languageId = null, ICollection<string>? pagesIds = null, ICollection<string>? commentsIds = null,
            bool? isApproved = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
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

            await dbContext.SaveChangesAsync();
            return chapter;
        }

        public async Task<bool> Delete(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var chapter = await dbContext.Chapters.Where(c => c.Id == id).Include(c => c.Pages).Include(c => c.Comments).
                FirstOrDefaultAsync() ?? throw new NotFoundException(nameof(Chapter));
            dbContext.Chapters.Remove(chapter);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ChapterRequest> GetRequest(string requestId)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.OfType<ChapterRequest>().Where(r => r.Id == requestId)
                .Include(r => r.Chapter).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(ChapterRequest));
            return request;
        }

        public async Task<ChapterRequest> SubmitRequest(string chapterId, string groupId, string note)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var chapter = await dbContext.Chapters.FirstOrDefaultAsync(t => t.Id == chapterId) ??
                throw new NotFoundException(nameof(Chapter));
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId) ??
                throw new NotFoundException(nameof(Group));
            group.CheckUploadContions();

            var request = new ChapterRequest
            {
                Chapter = chapter,
                Group = group,
                SubmitNote = note,
                IsApproved = false,
                IsReviewed = false
            };

            await dbContext.Requests.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<ChapterRequest> ReviewRequest(string requestId, string note, bool isApproved)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
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
    }
}
