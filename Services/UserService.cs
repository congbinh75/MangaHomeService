using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MangaHomeService.Services
{
    public class UserService : IUserService
    {
        private string testEmail = "hehexd";
        private string testPassword = "123";
        public UserService() { }

        public async Task<User> Get(string email, string password)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                if (email.Equals(testEmail) && password.Equals(testPassword)) 
                {
                    return new User();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
