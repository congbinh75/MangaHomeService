﻿using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MangaHomeService.Services
{
    public class PageService : IPageService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;
        private readonly IConfiguration _configuration;
        public PageService(IDbContextFactory<MangaHomeDbContext> contextFactory, IConfiguration configuration) 
        {
            _contextFactory = contextFactory;
            _configuration = configuration;
        }

        public async Task<Page> Add(string chapterId, int number, IFormFile file)
        {
            using (var dbContext = _contextFactory.CreateDbContext()) 
            {
                if (file == null)
                {
                    throw new NullReferenceException(nameof(file));
                }

                var chapter = await dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == chapterId);
                if (chapter == null)
                {
                    throw new NullReferenceException(nameof(chapter));
                }

                var existingNumberPage = await dbContext.Pages.FirstOrDefaultAsync(p => p.Chapter.Id == chapterId && p.Number == number);
                if (existingNumberPage != null) 
                {
                    throw new ArgumentException();
                }

                string filePath = await UploadPageAsync(file);

                var page = new Page();
                page.Chapter = chapter;
                page.Number = number;
                page.File = filePath;
                await dbContext.Pages.AddAsync(page);
                await dbContext.SaveChangesAsync();
                return page;
            }
        }

        public async Task<bool> Delete(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync()) 
            {
                var page = await dbContext.Pages.FirstOrDefaultAsync(c => c.Id == id);
                if (page == null)
                {
                    throw new NullReferenceException(nameof(page));
                }
                dbContext.Pages.Remove(page);
                dbContext.SaveChanges();
                return true;
            }
        }

        public async Task<Page> Get(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var page = await dbContext.Pages.FirstOrDefaultAsync(c => c.Id == id);
                if (page == null)
                {
                    throw new NullReferenceException(nameof(page));
                }
                return page;
            }
        }

        public async Task<List<Page>> GetByChapter(string chapterId)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var pages = await dbContext.Pages.Where(c => c.Chapter.Id == chapterId).ToListAsync();
                return pages;
            }
        }

        public async Task<Page> Update(string id, string chapterId = "", int number = 0, IFormFile? file = null)
        {
            using (var dbContext = _contextFactory.CreateDbContext()) 
            {
                var page = await dbContext.Pages.FirstOrDefaultAsync(p => p.Id == id);
                if (page == null) 
                {
                    throw new NullReferenceException(nameof(page));
                }

                if (chapterId != "")
                {
                    var chapter = await dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == chapterId);
                    if (chapter == null)
                    {
                        throw new NullReferenceException(nameof(Chapter));
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
                    filePath = await UploadPageAsync(file);
                    page.File = filePath;
                }

                await dbContext.SaveChangesAsync();
                return page;
            }
        }

        private async Task<string> UploadPageAsync(IFormFile file)
        {
            var filePath = Path.Combine(_configuration["PagesStoragePath"], Path.GetRandomFileName());
            using (var stream = File.OpenRead(filePath))
            {
                await file.CopyToAsync(stream);
                return filePath;
            }
        }
    }
}