using MangaHomeService.Models;

namespace MangaHomeService.Services.Interfaces
{
    public interface IRoleService
    {
        Task<Role?> Get(string id);
        Task<List<Role>> GetAll();
        Task<Role> Add(string name, string? description = null, List<string>? permissionsIds = null);
        Task<Role> Update(string id, string? name = null, string? description = null, List<string>? permissionsIds = null);
        Task<bool> Remove(string id);
    }
}
