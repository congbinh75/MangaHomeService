using MangaHomeService.Models.Entities;

namespace MangaHomeService.Services
{
    public interface IPersonService
    {
        public Task<Person> Get(string id);
        public Task<Person> Add(string name, IFormFile? image = null, string? description = null,
            ICollection<string>? authoredTitlesIds = null, ICollection<string>? illustratedTitlesIds = null);
        public Task<Person> Update(string id, string? name = null, IFormFile? image = null, string? description = null,
            ICollection<string>? authoredTitlesIds = null, ICollection<string>? illustratedTitlesIds = null);
        public Task<bool> Remove(string id);
    }
}