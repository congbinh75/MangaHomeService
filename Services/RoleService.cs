using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public class RoleService: IRoleService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public RoleService(IDbContextFactory<MangaHomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Role?> Get(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync()) 
            {
                return await dbContext.Roles.Where(r => r.Id == id).FirstOrDefaultAsync();
            }
        }

        public async Task<List<Role>> GetAll()
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                return await dbContext.Roles.ToListAsync();
            }
        }

        public async Task<Role> Add(string name, string description)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                Role role = new Role();
                role.Name = name;
                role.Description = description;
                await dbContext.Roles.AddAsync(role);
                await dbContext.SaveChangesAsync();
                return role;
            }
        }

        public async Task<Role> Update(string id, string? name = null, string? description = null, List<string>? permissionIds = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
                if (role == null) 
                {
                    throw new ArgumentException(nameof(id));
                }

                var permissions = role.Permissions.ToList();
                if (permissionIds != null)
                {
                    permissions = await dbContext.Permissions.
                        Where(p => permissionIds.Distinct().Contains(p.Id)).ToListAsync();
                    if (permissions.Count() != permissionIds.Distinct().Count())
                    {
                        throw new ArgumentException(nameof(permissionIds));
                    }
                }

                role.Permissions = permissions;
                role.Name = name != null ? name : role.Name;
                role.Description = description != null ? description : role.Description;
                await dbContext.SaveChangesAsync();
                return role;
            }
        }

        public async Task<bool> Remove(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
                if (role == null)
                {
                    throw new ArgumentException(nameof(id));
                }
                dbContext.Roles.Remove(role);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }
    }
}
