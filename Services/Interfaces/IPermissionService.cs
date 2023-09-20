using MangaHomeService.Models;

namespace MangaHomeService.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<Permission?> Get(string id);
        Task<List<Permission>> GetAll();
        Task Add(string name, string description);
        Task Update(string id, string name, string description);
        Task Remove(string id);
    }
}
