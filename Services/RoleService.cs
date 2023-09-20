using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

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
            using (var dbContext = _contextFactory.CreateDbContext()) 
            {
                return await dbContext.Roles.Where(r => r.Id == id).FirstOrDefaultAsync();
            }
        }

        public async Task<List<Role>> GetAll()
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                return await dbContext.Roles.ToListAsync();
            }
        }

        public async Task Add(string name, string description)
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

        public async Task Update(string id, string name, string description)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                if (id == null)
                {
                    throw new Exception();
                }
                var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
                if (role != null) 
                {
                    role.Name = name;
                    role.Description = description;
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
                var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
                if (role != null)
                {
                    dbContext.Roles.Remove(role);
                    await dbContext.SaveChangesAsync();
                }    
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
    }
}
