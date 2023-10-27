using MangaHomeService.Models;

namespace MangaHomeService.Services.Interfaces
{
    public interface IPageService
    {
        public Task<Page> Get(string id);
        public Task<List<Page>> GetByChapter(string chapterId);
        public Task<Page> Add(string chapterId, int number, IFormFile file);
        public Task<Page> Update(string id, string chapterId = "", int number = 0, IFormFile? file = null);
        public Task<bool> Delete(string id);
    }
}
