using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MangaHomeService.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public UserService(IDbContextFactory<MangaHomeDbContext> contextFactory) 
        {
            _contextFactory = contextFactory;
        }

        public async Task<User> Add(string name, string email, string password, int role)
        {
            try
            {
                using (var dbContext = _contextFactory.CreateDbContext()) 
                {
                    User? existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
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

                    await dbContext.Users.AddAsync(newUser);
                    await dbContext.SaveChangesAsync();
                    return newUser;
                }
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User?> Get(string email, string password)
        {
            try
            {
                using (var dbContext = _contextFactory.CreateDbContext())
                {
                    User? user = await dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        string hassed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: password,
                            salt: user.Salt ?? new byte[8],
                            prf: KeyDerivationPrf.HMACSHA256,
                            iterationCount: 100000,
                            numBytesRequested: 256 / 8));
                        if (hassed.Equals(user.Password))
                        {
                            return user;
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message); 
            }
        }

        public async Task UpdateProfilePicture(string userId, string profilePicture)
        {
            try
            {
                using (var dbContext = _contextFactory.CreateDbContext())
                {
                    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    if (user == null) 
                    {
                        throw new Exception();
                    }
                    user.ProfilePicture = profilePicture;
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
