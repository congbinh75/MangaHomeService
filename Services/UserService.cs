using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public UserService(IDbContextFactory<MangaHomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<User> Add(string name, string email, string password, string roleId)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (existingUser != null)
                {
                    throw new Exception("Email already registered");
                }

                var role = await dbContext.Roles.Where(r => r.Id == roleId).FirstOrDefaultAsync();
                if (role == null)
                {
                    throw new Exception("Role not found");
                }

                (string hashed, byte[] salt) passAndSalt = HashPassword(password);
                User newUser = new User(
                    name, email, passAndSalt.hashed, emailConfirmed: false, profilePicture: "", passAndSalt.salt, role);
                await dbContext.Users.AddAsync(newUser);
                await dbContext.SaveChangesAsync();
                return newUser;
            }
        }

        public async Task<User?> Get(string id)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                User? user = await dbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
                return user;
            }
        }

        public async Task<User?> Get(string email, string password)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                User? user = await dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
                if (user != null && HashPassword(password, user.Salt).hashedPassword.Equals(user.Password))
                {
                    return user;
                }
                return null;
            }
        }

        public async Task<bool> Update(string userId, string? name = null, string? email = null, string? password = null,
            bool? emailConfirmed = null, string? profilePicture = null, string? roleName = null)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                var role = await dbContext.Roles.Where(r => r.Name == roleName).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new Exception();
                }

                var newName = name == null ? user.Name : name;
                var newEmail = email == null ? user.Email : email;
                var newRole = roleName == null ? user.Role : role;
                var newEmailConfirmed = emailConfirmed == null ? user.EmailConfirmed : emailConfirmed;
                var newProfilePicture = profilePicture == null ? user.ProfilePicture : profilePicture;

                var newPassword = user.Password;
                var newSalt = user.Salt;
                if (password != null)
                {
                    (string hassed, byte[] salt) passAndSalt = HashPassword(password);
                    newPassword = passAndSalt.hassed;
                    newSalt = passAndSalt.salt;
                }

                user.Name = newName;
                user.Email = email;
                user.Role = newRole;
                user.EmailConfirmed = (bool)newEmailConfirmed;
                user.ProfilePicture = newProfilePicture;
                user.Password = newPassword;
                user.Salt = newSalt;

                await dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> Delete(string id)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    throw new NullReferenceException(nameof(user));
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

        public async Task<List<Permission>> GetPermissionsOfUser(string userId)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception();
                }
                else
                {
                    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    var role = await dbContext.Roles.Where(r => r.Id == user.Role.Id).Include(r => r.Permissions).FirstOrDefaultAsync();
                    return role.Permissions;
                }
            }
        }
    }
}
