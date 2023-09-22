using MangaHomeService.Models;

namespace MangaHomeService.Services.Interfaces
{
    public interface IRoleService
    {
        Task<Role?> Get(string id);
        Task<List<Role>> GetAll();
        Task<Role> Add(string name, string description);
        Task<Role> Update(string id, string name, string description);
        Task<bool> Remove(string id);
        Task<Role> UpdatePermissionsOfRole(string roleId, List<string> permissionIds);
    }
}
