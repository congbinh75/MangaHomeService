using MangaHomeService.Models;

namespace MangaHomeService.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<Permission?> Get(string id);
        Task<List<Permission>> GetAll();
        Task<Permission> Add(string name, string description, List<string>? rolesIds = null);
        Task<Permission> Update(string id, string name, string? description = null, List<string>? rolesIds = null);
        Task<bool> Remove(string id);
    }
}
