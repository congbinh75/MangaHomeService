using MangaHomeService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MangaHomeService.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Add(string name, string email, string password, string? roleName = null);
        Task<User?> Get(string id);
        Task<User?> Get(string email, string password);
        Task Update(string id, string? name = null, string? email = null, string? password = null, bool? emailConfirmed = null, 
            string? profilePicture = null, string? roleName = null);
        Task Delete(string id);
        Task AddRole(string name, string description);
        Task UpdateRole (string id, string name, string description);
        Task RemoveRole(string id);
        Task AddPermission(string name, string description);
        Task UpdatePermission(string id, string name, string description);
        Task RemovePermission(string id);
        Task UpdateRolesPermissions(string roleId, List<string> permissionIds);
        Task<List<Permission>> GetPermissions(string userId);
    }
}
