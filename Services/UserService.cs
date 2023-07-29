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
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                var role = await dbContext.Roles.Where(r => r.Name == roleName).FirstOrDefaultAsync();
                if (existingUser != null)
                {
                    throw new Exception("Email already registered");
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

        public async Task Update(string userId, string? name = null, string? email = null, string? password = null,
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
            }
        }

        public async Task Delete(string id)
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

        public async Task AddRole(string name, string description)
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

        public async Task UpdateRole(string id, string name, string description)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                if (id == null)
                {
                    throw new Exception();
                }
                var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
                role.Name = name;
                role.Description = description;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveRole(string id)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                if (id == null)
                {
                    throw new Exception();
                }
                var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
                dbContext.Roles.Remove(role);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task AddPermission(string name, string description)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                Permission permission = new Permission();
                permission.Name = name;
                permission.Description = description;
                await dbContext.Permissions.AddAsync(permission);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdatePermission(string id, string name, string description)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                if (id == null)
                {
                    throw new Exception();
                }
                var permission = await dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == id);
                permission.Name = name;
                permission.Description = description;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task RemovePermission(string id)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                if (id == null)
                {
                    throw new Exception();
                }
                var permission = await dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == id);
                dbContext.Permissions.Remove(permission);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateRolesPermissions(string roleId, List<string> permissionIds)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                var permissions = await dbContext.Permissions.
                    Where(p => permissionIds.Distinct().Contains(p.Id)).ToListAsync();
                if (permissions.Count() != permissionIds.Distinct().Count())
                {
                    throw new Exception();
                }

                var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
                if (role == null)
                {
                    throw new Exception();
                }

                role.Permissions = permissions;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Permission>> GetPermissions(string userId)
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
