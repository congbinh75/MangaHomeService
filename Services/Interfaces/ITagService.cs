using MangaHomeService.Models.Entities;

namespace MangaHomeService.Services
{
    public interface ITagService
    {
        public Task<Tag> Get(string id);
        public Task<ICollection<Tag>> GetByType(Type type);
        public Task<Tag> Add(string name, Type type, string? description = null, ICollection<string>? titlesIds = null,
            ICollection<string>? otherNamesIds = null);
        public Task<Tag> Update(string id, string? name = null, string? description = null, ICollection<string>? titlesIds = null,
            ICollection<string>? otherNamesIds = null);
        public Task<bool> Remove(string id);
    }
}