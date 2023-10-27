using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public PermissionService(IDbContextFactory<MangaHomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Permission?> Get(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                return await dbContext.Permissions.Where(r => r.Id == id).FirstOrDefaultAsync();
            }
        }

        public async Task<List<Permission>> GetAll()
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                return await dbContext.Permissions.ToListAsync();
            }
        }

        public async Task<Permission> Add(string name, string description)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                Permission permission = new Permission();
                permission.Name = name;
                permission.Description = description;
                await dbContext.Permissions.AddAsync(permission);
                await dbContext.SaveChangesAsync();
                return permission;
            }
        }

        public async Task<Permission> Update(string id, string name, string description)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var permission = await dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == id);
                if (permission == null)
                {
                    throw new ArgumentException(nameof(id));
                }
                permission.Name = name;
                permission.Description = description;
                await dbContext.SaveChangesAsync();
                return permission;
            }
        }

        public async Task<bool> Remove(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var permission = await dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == id);
                if (permission == null)
                {
                    throw new ArgumentException(nameof(id));
                }
                dbContext.Permissions.Remove(permission);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }
    }
}
