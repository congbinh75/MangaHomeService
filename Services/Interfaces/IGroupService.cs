using MangaHomeService.Models.Entities;
using MangaHomeService.Utils;

namespace MangaHomeService.Services
{
    public interface IGroupService
    {
        public Task<Group> Get(string id);
        public Task<ICollection<Group>> GetAll(int pageSize = Constants.GroupsPerPage, int pageNumber = 1);
        public Task<Group> Add(string name, string? description = null, IFormFile? profilePicture = null,
            ICollection<string>? membersIds = null);
        public Task<Group> Update(string id, string? name = null, string? description = null, IFormFile? profilePicture = null,
            ICollection<string>? membersIds = null);
        public Task<bool> Remove(string id);
    }
}