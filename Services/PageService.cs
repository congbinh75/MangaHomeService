using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public class PageService : IPageService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;
        public PageService(IDbContextFactory<MangaHomeDbContext> contextFactory) 
        {
            _contextFactory = contextFactory;
        }

        public async Task<Page> Add(string chapterId, int number, IFormFile file)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Page> Get(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Page>> GetByChapter(string chapterId)
        {
            throw new NotImplementedException();
        }

        public async Task<Page> Update(string id, string chapterId = "", int number = -1, IFormFile? file = null)
        {
            throw new NotImplementedException();
        }
    }
}
