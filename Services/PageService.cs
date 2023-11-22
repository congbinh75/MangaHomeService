using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public interface IPageService
    {
        public Task<Page> Get(string id);
        public Task<ICollection<Page>> GetByChapter(string chapterId);
        public Task<Page> Add(string chapterId, int number, IFormFile file);
        public Task<Page> Update(string id, string chapterId = "", int number = 0, IFormFile? file = null);
        public Task<bool> Delete(string id);
    }

    public class PageService : IPageService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;
        private readonly IConfiguration _configuration;
        public PageService(IDbContextFactory<MangaHomeDbContext> contextFactory, IConfiguration configuration)
        {
            _contextFactory = contextFactory;
            _configuration = configuration;
        }

        public async Task<Page> Get(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var page = await dbContext.Pages.FirstOrDefaultAsync(c => c.Id == id) ?? throw new NotFoundException(typeof(Page).Name);
            return page;
        }

        public async Task<ICollection<Page>> GetByChapter(string chapterId)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var pages = await dbContext.Pages.Where(c => c.Chapter != null && c.Chapter.Id == chapterId).ToListAsync();
            return pages;
        }

        public async Task<Page> Add(string chapterId, int number, IFormFile file)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                if (file == null)
                {
                    throw new ArgumentException(nameof(file));
                }

                var chapter = await dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == chapterId) ??
                    throw new NotFoundException(typeof(Chapter).Name);
                var existingNumberPage = await dbContext.Pages.FirstOrDefaultAsync(p => (p.Chapter != null && p.Chapter.Id == chapterId) 
                    && p.Number == number);
                if (existingNumberPage != null)
                {
                    throw new ArgumentException();
                }

                string filePath = await Functions.UploadFileAsync(file, _configuration["FilesStoragePath.PagesPath"]);

                var page = new Page
                {
                    Chapter = chapter,
                    Number = number,
                    File = filePath
                };
                await dbContext.Pages.AddAsync(page);
                await dbContext.SaveChangesAsync();
                return page;
            }
        }

        public async Task<Page> Update(string id, string chapterId = "", int number = 0, IFormFile? file = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var page = await dbContext.Pages.FirstOrDefaultAsync(p => p.Id == id) ??
                throw new NotFoundException(typeof(Page).Name);
            if (chapterId != "")
            {
                var chapter = await dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == chapterId);
                if (chapter == null)
                {
                    throw new NotFoundException(typeof(Chapter).Name);
                }
                else
                {
                    page.Chapter = chapter;
                }
            }

            if (number > 0)
            {
                var existingPage = await dbContext.Pages.FirstOrDefaultAsync(p => (p.Chapter != null && p.Chapter.Id == chapterId) && p.Number == number);
                if (existingPage != null)
                {
                    throw new ArgumentException();
                }
                else
                {
                    page.Number = number;
                }
            }

            string filePath = "";
            if (file != null)
            {
                filePath = await Functions.UploadFileAsync(file, _configuration["FilesStoragePath.PagesPath"]);
                page.File = filePath;
            }

            await dbContext.SaveChangesAsync();
            return page;
        }

        public async Task<bool> Delete(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var page = await dbContext.Pages.FirstOrDefaultAsync(c => c.Id == id) ??
                throw new NotFoundException(typeof(Page).Name);
            dbContext.Pages.Remove(page);
            dbContext.SaveChanges();
            return true;
        }
    }
}
