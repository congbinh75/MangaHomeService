using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;
using Group = MangaHomeService.Models.Group;

namespace MangaHomeService.Services
{
    public interface IGroupService
    {
        public Task<Group> Get(string id);
        public Task<ICollection<Group>> GetAll(int pageSize = Constants.GroupsPerPage, int pageNumber = 1);
        public Task<Group> Add(string name, string? description = null, IFormFile? profilePicture = null, 
            ICollection<string>? membersIds = null);
        public Task<Group> Update(string id, string? name = null, string? description = null, IFormFile? profilePicture = null,
            ICollection<string>? membersIds = null);
        public Task<bool> Remove(string id);
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
                throw new NotFoundException(nameof(Group));
            return group;
        }

        public async Task<ICollection<Group>> GetAll(int pageSize = Constants.GroupsPerPage, int pageNumber = 1)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var groups = await dbContext.Groups.Where(x => x.IsApproved == true).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
            return groups;
        }

        public async Task<Group> Add(string name, string? description = null, IFormFile? profilePicture = null, 
            ICollection<string>? membersIds = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
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
                (await Functions.UploadFileAsync(profilePicture, _configuration["FilesStoragePath.GroupsImagesPath"]
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
            using var dbContext = await _contextFactory.CreateDbContextAsync();
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
                await Functions.UploadFileAsync(profilePicture, _configuration["FilesStoragePath.GroupsImagesPath"]
                ?? throw new ConfigurationNotFoundException("FilesStoragePath.GroupsImagesPath"));
            group.Members = newMembers;
            await dbContext.SaveChangesAsync();
            return group;
        }

        public async Task<bool> Remove(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == id) ??
                throw new NotFoundException(nameof(Group));
            dbContext.Groups.Remove(group);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<GroupRequest> GetRequest(string requestId)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.OfType<GroupRequest>().Where(r => r.Id == requestId).
                Include(r => r.Group).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(GroupRequest));
            return request;
        }

        public async Task<GroupRequest> SubmitRequest(string groupId, string note)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId) ??
                throw new NotFoundException(nameof(Group));
            if (group.IsApproved)
            {
                throw new AlreadyApprovedException(nameof(Group));
            }
            var request = new GroupRequest
            {
                Group = group,
                SubmitNote = note,
                IsApproved = false,
                IsReviewed = false
            };
            await dbContext.Requests.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<GroupRequest> ReviewRequest(string requestId, string note, bool isApproved)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.OfType<GroupRequest>().FirstOrDefaultAsync(g => g.Id == requestId) ??
                throw new NotFoundException(nameof(GroupRequest));
            if (request.IsReviewed)
            {
                throw new AlreadyReviewedException(nameof(GroupRequest));
            }
            request.ReviewNote = note;
            request.IsReviewed = true;
            request.IsApproved = isApproved;
            await dbContext.SaveChangesAsync();
            return request;
        }
    }
}
