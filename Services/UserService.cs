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

        public async Task<User> Add(string name, string email, string password, string roleName)
        {
            try
            {
                using (var dbContext = _contextFactory.CreateDbContext()) 
                {
                    User? existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                    Role? role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                    if (existingUser != null)
                    {
                        throw new Exception("Email already registered");
                    }

                    (string hashed, byte[] salt) passAndSalt = HashPassword(password);
                    User newUser = new User(
                        name, email, passAndSalt.hashed, emailConfirmed: false, profilePicture : "", passAndSalt.salt, role);
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

        public async Task<User?> Get(string id)
        {
            try
            {
                using (var dbContext = _contextFactory.CreateDbContext())
                {
                    User? user = await dbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
                    return user;
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
                    if (user != null && HashPassword(password, user.Salt).hashedPassword.Equals(user.Password))
                    {
                        return user;
                    }
                    return null;
                }
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message); 
            }
        }

        public async Task Update(string userId, string? name = null, string? email = null, string? password = null, 
            bool? emailConfirmed = null, string? profilePicture = null, string? roleName = null)
        {
            try
            {
                using (var dbContext = _contextFactory.CreateDbContext())
                {
                    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
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
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                using (var dbContext = _contextFactory.CreateDbContext())
                {
                    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
                    if (user != null) 
                    {
                        dbContext.Users.Remove(user);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddRole(string name, string description)
        {
            try
            {
                using (var dbContext = _contextFactory.CreateDbContext())
                {
                    Role role = new Role();
                    role.Name = name;
                    role.Description = description;
                    await dbContext.Roles.AddAsync(role);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
