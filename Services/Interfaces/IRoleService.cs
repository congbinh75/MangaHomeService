using MangaHomeService.Models;

namespace MangaHomeService.Services.Interfaces
{
    public interface IRoleService
    {
        Task<Role?> Get(string id);
        Task<List<Role>> GetAll(bool inactiveIncluded = false);
        Task Add(string name, string description);
        Task Update(string id, string name, bool isActive, string description);
        Task Remove(string id);
        Task UpdateRolesPermissions(string roleId, List<string> permissionIds);
    }
}
