using MangaHomeService.Models.Entities;

namespace MangaHomeService.Services
{
    public interface IReadingListService
    {
        public Task<ReadingList> Get(string id);
        public Task<ICollection<ReadingList>> GetAll(string? userId = null);
        public Task<ReadingList> Add(string name, string? userId = null, string description = "", bool isPublic = false,
            ICollection<string>? titlesIds = null);
        public Task<ReadingList> Update(string id, string? userId = null, string? name = null, string? description = null,
            bool? isPublic = null, ICollection<string>? titlesIds = null);
        public Task<bool> Remove(string id);
        public Task<ReadingList> AddTitle(string id, string titleId);
        public Task<ReadingList> RemoveTitle(string id, string titleId);
    }
}