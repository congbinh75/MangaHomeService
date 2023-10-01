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

        public async Task<Chapter> Add(double number, Title title, Group group, Volume? volume = null, Language? language = null, 
            List<Page>? pages = null, List<Comment>? comments = null, bool? isApproved = null)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
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
            using (var dbContext = _contextFactory.CreateDbContext()) 
            {
                var chapter = await dbContext.Chapters.Where(c => c.Id == id).Include(c => c.Pages).Include(c => c.Comments).
                    FirstOrDefaultAsync();
                if (chapter == null) 
                {
                    throw new NullReferenceException(nameof(chapter));
                }
                dbContext.Chapters.Remove(chapter);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<Chapter?> Get(string id)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                return await dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == id);
            }
        }

        public async Task<List<Chapter>> GetByTitle(string titleId)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                return await dbContext.Chapters.Where(c => c.Title.Id == titleId).OrderByDescending(c => c.Number).ToListAsync();
            }
        }

        public async Task<Chapter> Update(string id, double number = -1, Title? title = null, Group? group = null, Volume? volume = null, 
            Language? language = null, List<Page>? pages = null, List<Comment>? comments = null)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                var chapter = await dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == id);
                if (chapter == null)
                {
                    throw new NullReferenceException(nameof(chapter));
                }

                var newNumber = number > 0 ? number : chapter.Number;
                var newTitle = title != null ? title : chapter.Title;
                var newGroup = group != null ? group : chapter.Group;
                var newVolume = volume != null ? volume : chapter.Volume;
                var newLanguage = language != null ? language : chapter.Language;
                var newPages = pages != null ? pages : chapter.Pages;
                var newComments = comments != null ? comments : chapter.Comments;

                chapter.Number = newNumber;
                chapter.Title = newTitle;
                chapter.Group = newGroup;
                chapter.Volume = newVolume;
                chapter.Language = newLanguage;
                chapter.Pages = newPages;
                chapter.Comments = newComments;
                chapter.IsApproved = null;

                await dbContext.SaveChangesAsync();
                return chapter;
            }
        }

        public async Task<Tuple<Chapter, ChapterRequest>> Submit(double number, Title title, Group group, Volume? volume = null, Language? language = null,
            List<Page>? pages = null, List<Comment>? comments = null)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                var chapter = await Add(number, title, group, volume, language, pages, comments);
                var submitUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == Functions.GetCurrentUserId());
                var request = new ChapterRequest();
                request.Chapter = chapter;
                request.User = submitUser;
                request.Group = group;
                request.IsApproved = null;

                await dbContext.ChapterRequests.AddAsync(request);
                await dbContext.SaveChangesAsync();
                return(Tuple.Create(chapter, request));
            }
        }

        public async Task<Tuple<Chapter, ChapterRequest>> ApproveOrRejectRequest(string requestId, bool isApproved)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                var request = await dbContext.ChapterRequests.Where(r => r.Id == requestId).Include(r => r.Chapter).FirstOrDefaultAsync();
                if (request.IsApproved != null || request.Chapter.IsApproved != null)
                {
                    throw new ArgumentException();
                }
                
                request.IsApproved = isApproved;
                request.Chapter.IsApproved = isApproved;
                await dbContext.SaveChangesAsync();
                return (Tuple.Create(request.Chapter, request));
            }
        }
    }
}
