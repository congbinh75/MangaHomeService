using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Data;
using MangaHomeService.Utils;

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
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (existingUser != null)
                {
                    throw new EmailAlreadyRegisteredException();
                }

                (string hashed, byte[] salt) passAndSalt = HashPassword(password);
                User newUser = new User();
                newUser.Name = name;
                newUser.Email = email;
                newUser.Password = passAndSalt.hashed;
                newUser.IsEmailConfirmed = false;
                newUser.ProfilePicture = "";
                newUser.Salt = passAndSalt.salt;
                newUser.Role = role;
                newUser.IsBanned = false;
                await dbContext.Users.AddAsync(newUser);
                await dbContext.SaveChangesAsync();
                return newUser;
            }
        }

        public async Task<User> Get(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                User? user = await dbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new ArgumentException(nameof(id));
                }
                return user;
            }
        }

        public async Task<User?> Get(string email, string password)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                User? user = await dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
                if (user != null && HashPassword(password, user.Salt).hashedPassword.Equals(user.Password))
                {
                    return user;
                }
                else
                {
                    throw new InvalidCredentialsException();
                }
            }
        }

        public async Task<User> Update(string userId, string? name = null, string? email = null, string? password = null, int? role = null,
            bool? isEmailConfirmed = null, string? profilePicture = null, bool? isBanned = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    throw new NotFoundException(typeof(User).ToString());
                }

                var newPassword = user.Password;
                var newSalt = user.Salt;
                if (password != null)
                {
                    (string hassed, byte[] salt) passAndSalt = HashPassword(password);
                    newPassword = passAndSalt.hassed;
                    newSalt = passAndSalt.salt;
                }

                user.Name = name == null ? user.Name : name;
                user.Email = email == null ? user.Email : email;
                user.Role = role == null ? user.Role : (int)role;
                user.IsEmailConfirmed = isEmailConfirmed == null ? user.IsEmailConfirmed : (bool)isEmailConfirmed;
                user.ProfilePicture = profilePicture == null ? user.ProfilePicture : profilePicture;
                user.Password = newPassword;
                user.Salt = newSalt;
                user.IsBanned = isBanned == null ? user.IsBanned : (bool)isBanned;

                await dbContext.SaveChangesAsync();
                return user;
            }
        }

        public async Task<bool> Delete(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    throw new NotFoundException(typeof(User).ToString());
                }
                dbContext.Users.Remove(user);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }

        private (string hashedPassword, byte[] salt) HashPassword(string password, byte[]? salt = null)
        {
            if (salt == null)
            {
                salt = RandomNumberGenerator.GetBytes(128 / 8);
            }
            string hassed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
            return (hassed, salt);
        }
    }
}
