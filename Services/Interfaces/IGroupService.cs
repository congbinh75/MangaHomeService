using MangaHomeService.Models;

namespace MangaHomeService.Services.Interfaces
{
    public interface IGroupService
    {
        public Task<Group> Get(string id);
        
    }
}
