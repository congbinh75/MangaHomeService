using MangaHomeService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MangaHomeService.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Add(string name, string email, string password, int role);
        Task<User?> Get(string email, string password);
        Task UpdateProfilePicture(string userId, string picture);
    }
}
