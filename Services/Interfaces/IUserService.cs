using MangaHomeService.Models;

namespace MangaHomeService.Services.Interfaces
{
    public interface IUserService
    {
        public Task<User> Add(string name, string email, string password, int role);
        public Task<User> Get(string id);
        public Task<User?> Get(string email, string password);
        public Task<User> Update(string id, string? name = null, string? email = null, string? password = null, int? role = null,
            bool ? emailConfirmed = null, string? profilePicture = null);
        public Task<bool> Delete(string id);
    }
}
