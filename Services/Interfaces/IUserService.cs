using MangaHomeService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MangaHomeService.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Add(string name, string email, string password, List<string>? roleNames = null);
        Task<User?> Get(string id);
        Task<User?> Get(string email, string password);
        Task Update(string id, string? name = null, string? email = null, string? password = null, bool? emailConfirmed = null, 
            string? profilePicture = null, List<string>? roleNames = null);
        Task Delete(string id);
        Task AddRole(string name, string description);
    }
}
