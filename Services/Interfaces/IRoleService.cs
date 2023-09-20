using MangaHomeService.Models;

namespace MangaHomeService.Services.Interfaces
{
    public interface IRoleService
    {
        Task<Role?> Get(string id);
        Task<List<Role>> GetAll();
        Task Add(string name, string description);
        Task Update(string id, string name, string description);
        Task Remove(string id);
        Task UpdateRolesPermissions(string roleId, List<string> permissionIds);
    }
}
