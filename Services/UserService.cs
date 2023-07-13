using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public class UserService : IUserService
    {
        private MangaHomeDbContext _dbContext;
        public UserService(MangaHomeDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<User> Add(string name, string email, string password, int role)
        {
            try
            {
                User? existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
                if (existingUser != null) 
                {
                    throw new Exception("Email already registered");
                }

                byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
                string hassedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                User newUser = new User(name, email, hassedPassword, role);
                newUser.Salt = salt;
                newUser.EmailConfirmed = false;
                newUser.ProfilePicture = "";

                await _dbContext.Users.AddAsync(newUser);
                await _dbContext.SaveChangesAsync();
                return newUser;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> Get(string email, string password)
        {
            try
            {
                User? user = await _dbContext.Users.Where(u => u.Email.Equals(email)).FirstOrDefaultAsync();
                if (user != null)
                {
                    string hassed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: password,
                        salt: user.Salt,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 256 / 8));
                    if (hassed.Equals(user.Password))
                    {
                        return user;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message); 
            }
        }
    }
}
