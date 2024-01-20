using MangaHomeService.Models;
using MangaHomeService.Models.Entities;
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
        public Task<bool> Remove(string id);
    }

    public class PageService(IDbContextFactory<MangaHomeDbContext> contextFactory, IConfiguration configuration) : IPageService
    {
        public async Task<Page> Get(string id)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var page = await dbContext.Pages.FirstOrDefaultAsync(c => c.Id == id) ?? throw new NotFoundException(nameof(Page));
            return page;
        }

        public async Task<ICollection<Page>> GetByChapter(string chapterId)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var pages = await dbContext.Pages.Where(c => c.Chapter.Id == chapterId).ToListAsync();
            return pages;
        }

        public async Task<Page> Add(string chapterId, int number, IFormFile file)
        {
            using (var dbContext = await contextFactory.CreateDbContextAsync())
            {
                var chapter = await dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == chapterId) ??
                    throw new NotFoundException(nameof(Chapter));
                var existingNumberPage = await dbContext.Pages.FirstOrDefaultAsync(p => p.Chapter.Id == chapterId && p.Number == number);
                if (existingNumberPage != null)
                {
                    throw new ArgumentException();
                }

                string filePath = await Functions.UploadFileAsync(file, configuration["FilesStoragePath.PagesPath"]);

                var page = new Page
                {
                    Chapter = chapter,
                    Number = number,
                    FilePath = filePath
                };
                await dbContext.Pages.AddAsync(page);
                await dbContext.SaveChangesAsync();
                return page;
            }
        }

        public async Task<Page> Update(string id, string chapterId = "", int number = 0, IFormFile? file = null)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var page = await dbContext.Pages.FirstOrDefaultAsync(p => p.Id == id) ??
                throw new NotFoundException(nameof(Page));
            if (chapterId != "")
            {
                var chapter = await dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == chapterId);
                if (chapter == null)
                {
                    throw new NotFoundException(nameof(Chapter));
                }
                else
                {
                    page.Chapter = chapter;
                }
            }

            if (number > 0)
            {
                var existingPage = await dbContext.Pages.FirstOrDefaultAsync(p => p.Chapter.Id == chapterId && p.Number == number);
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
                filePath = await Functions.UploadFileAsync(file, configuration["FilesStoragePath.PagesPath"]);
                page.FilePath = filePath;
            }

            await dbContext.SaveChangesAsync();
            return page;
        }

        public async Task<bool> Remove(string id)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var page = await dbContext.Pages.FirstOrDefaultAsync(c => c.Id == id) ??
                throw new NotFoundException(nameof(Page));
            dbContext.Pages.Remove(page);
            dbContext.SaveChanges();
            return true;
        }
    }
}
