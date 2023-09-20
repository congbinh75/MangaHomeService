using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

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
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                return await dbContext.Permissions.Where(r => r.Id == id).FirstOrDefaultAsync();
            }
        }

        public async Task<List<Permission>> GetAll()
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                return await dbContext.Permissions.ToListAsync();
            }
        }

        public async Task Add(string name, string description)
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

        public async Task Update(string id, string name, string description)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                if (id == null)
                {
                    throw new Exception();
                }
                var permission = await dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == id);
                if (permission != null)
                {
                    permission.Name = name;
                    permission.Description = description;
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task Remove(string id)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                if (id == null)
                {
                    throw new Exception();
                }
                var permission = await dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == id);
                if (permission != null)
                {
                    dbContext.Permissions.Remove(permission);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
