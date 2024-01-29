using MangaHomeService.Models.Entities;

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
}