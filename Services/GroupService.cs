using MangaHomeService.Models;
using MangaHomeService.Models.Entities;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;
using Group = MangaHomeService.Models.Entities.Group;

namespace MangaHomeService.Services
{
    public class GroupService(IDbContextFactory<MangaHomeDbContext> contextFactory, IConfiguration configuration) : IGroupService
    {
        public async Task<Group> Get(string id)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == id) ??
                throw new NotFoundException(nameof(Group));
            return group;
        }

        public async Task<ICollection<Group>> GetAll(int pageSize = Constants.GroupsPerPage, int pageNumber = 1)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var groups = await dbContext.Groups.Where(x => x.IsApproved == true).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
            return groups;
        }

        public async Task<Group> Add(string name, string? description = null, IFormFile? profilePicture = null,
            ICollection<string>? membersIds = null)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var members = new List<Member>();
            if (membersIds != null)
            {
                foreach (string memberId in membersIds)
                {
                    var member = await dbContext.Members.FirstOrDefaultAsync(m => m.Id == memberId) ??
                        throw new NotFoundException(nameof(Member));
                    members.Add(member);
                }
            }

            var group = new Group
            {
                Name = name,
                Description = description ?? "",
                ProfilePicture = profilePicture != null ?
                (await Functions.UploadFileAsync(profilePicture, configuration["FilesStoragePath.GroupsImagesPath"]
                ?? throw new ConfigurationNotFoundException("FilesStoragePath.GroupsImagesPath"))) : "",
                Members = members
            };
            await dbContext.Groups.AddAsync(group);
            await dbContext.SaveChangesAsync();
            return group;
        }

        public async Task<Group> Update(string id, string? name = null, string? description = null, IFormFile? profilePicture = null,
            ICollection<string>? membersIds = null)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == id) ??
                throw new NotFoundException(nameof(Group));

            var newMembers = new List<Member>();
            if (membersIds != null)
            {
                foreach (var memberId in membersIds)
                {
                    var member = await dbContext.Members.FirstOrDefaultAsync(m => m.Id == memberId) ??
                        throw new NotFoundException(nameof(Member));
                    newMembers.Add(member);
                }
            }
            else
            {
                newMembers = (List<Member>)group.Members;
            }

            group.Name = name ?? group.Name;
            group.Description = description ?? group.Description;
            group.ProfilePicture = profilePicture == null ? group.ProfilePicture :
                await Functions.UploadFileAsync(profilePicture, configuration["FilesStoragePath.GroupsImagesPath"]
                ?? throw new ConfigurationNotFoundException("FilesStoragePath.GroupsImagesPath"));
            group.Members = newMembers;
            await dbContext.SaveChangesAsync();
            return group;
        }

        public async Task<bool> Remove(string id)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == id) ??
                throw new NotFoundException(nameof(Group));
            dbContext.Groups.Remove(group);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
