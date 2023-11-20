using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;
using Group = MangaHomeService.Models.Group;

namespace MangaHomeService.Services
{
    public interface IGroupService
    {
        public Task<Group> Get(string id);
        public Task<Group> Add(string name, string description, IFormFile profilePicture, ICollection<string> membersIds);
        public Task<Group> Update(string id, string? name = null, string? description = null, IFormFile? profilePicture = null,
            ICollection<string>? membersIds = null);
        public Task<bool> Delete(string id);
        public Task<GroupRequest> GetRequest(string groupId);
        public Task<GroupRequest> SubmitRequest(string groupId, string note);
        public Task<GroupRequest> ReviewRequest(string requestId, string note, bool isApproved);
    }

    public class GroupService : IGroupService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;
        private readonly IConfiguration _configuration;

        public GroupService(IDbContextFactory<MangaHomeDbContext> contextFactory, IConfiguration configuration)
        {
            _contextFactory = contextFactory;
            _configuration = configuration; 
        }

        public async Task<Group> Get(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == id) ??
                throw new NotFoundException(typeof(Group).Name);
            return group;
        }

        public async Task<Group> Add(string name, string description, IFormFile profilePicture, ICollection<string> membersIds)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var members = new List<Member>();
            foreach (string memberId in membersIds)
            {
                var member = await dbContext.Members.FirstOrDefaultAsync(m => m.Id == memberId) ??
                    throw new NotFoundException(typeof(Member).Name);
                members.Add(member);
            }

            var group = new Group
            {
                Name = name,
                Description = description,
                ProfilePicture = await Functions.UploadFileAsync(profilePicture, _configuration["FilesStoragePath.GroupsImagesPath"]),
                Members = members
            };
            await dbContext.Groups.AddAsync(group);
            await dbContext.SaveChangesAsync();
            return group;
        }

        public async Task<Group> Update(string id, string? name = null, string? description = null, IFormFile? profilePicture = null,
            ICollection<string>? membersIds = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == id) ?? 
                throw new NotFoundException(typeof(Group).Name);

            var newMembers = new List<Member>();
            if (membersIds != null)
            {
                foreach (var memberId in membersIds)
                {
                    var member = await dbContext.Members.FirstOrDefaultAsync(m => m.Id == memberId) ?? 
                        throw new NotFoundException(typeof(Member).Name);
                    newMembers.Add(member);
                }
            }
            else
            {
                newMembers = group.Members;
            }

            group.Name = name ?? group.Name;
            group.Description = description ?? group.Description;
            group.ProfilePicture = profilePicture == null ? group.ProfilePicture :
                await Functions.UploadFileAsync(profilePicture, _configuration["FilesStoragePath.GroupsImagesPath"]);
            group.Members = newMembers;
            await dbContext.SaveChangesAsync();
            return group;
        }

        public async Task<bool> Delete(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == id) ?? 
                throw new NotFoundException(typeof(Group).Name);
            dbContext.Groups.Remove(group);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<GroupRequest> GetRequest(string groupId)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.GroupRequests.Where(r => r.Id == groupId).Include(r => r.Group).FirstOrDefaultAsync() ??
                throw new NotFoundException(typeof(ChapterRequest).Name);
            return request;
        }

        public async Task<GroupRequest> SubmitRequest(string groupId, string note)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId) ?? 
                throw new NotFoundException(typeof(Group).Name);
            var request = new GroupRequest
            {
                Group = group,
                SubmitNote = note,
                IsApproved = false,
                IsReviewed = false
            };
            await dbContext.GroupRequests.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<GroupRequest> ReviewRequest(string requestId, string note, bool isApproved)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.GroupRequests.FirstOrDefaultAsync(g => g.Id == requestId) ?? 
                throw new NotFoundException(typeof(GroupRequest).Name);
            request.ReviewNote = note;
            request.IsReviewed = true;
            request.IsApproved = isApproved;
            await dbContext.SaveChangesAsync();
            return request;
        }
    }
}
