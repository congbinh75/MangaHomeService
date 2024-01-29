using MangaHomeService.Models.Entities;

namespace MangaHomeService.Services
{
    public interface IUserService
    {
        public Task<User> Add(string name, string email, string password, int role);
        public Task<User> Get(string id);
        public Task<User?> Get(string email, string password);
        public Task<User> Update(string id, string? name = null, string? email = null, string? password = null, int? role = null,
            bool? isEmailConfirmed = null, IFormFile? profilePicture = null, bool? isBanned = null);
        public Task<bool> Remove(string id);
        public Task<bool> SendEmailConfirmation(string? userId = null);
        public Task<bool> ConfirmEmail(string userId, string token);
    }
}