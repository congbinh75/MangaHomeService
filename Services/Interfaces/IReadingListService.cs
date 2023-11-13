using MangaHomeService.Models;

namespace MangaHomeService.Services.Interfaces
{
    public interface IReadingListService
    {
        public Task<ReadingList> Get(string id);
        public Task<List<ReadingList>> GetAll(string? userId = null);
        public Task<ReadingList> Add(string name, string? userId = null, string description = "", bool isPublic = false, 
            List<string>? titlesIds = null);
        public Task<ReadingList> Update(string id, string? userId = null, string? name = null, string? description = null, 
            bool? isPublic = null, List<string>? titlesIds = null);
        public Task<bool> Delete(string id);
    }
}
