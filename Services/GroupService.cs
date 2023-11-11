using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;
using Group = MangaHomeService.Models.Group;

namespace MangaHomeService.Services
{
    public class GroupService : IGroupService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public GroupService(IDbContextFactory<MangaHomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<Group> Add(string name, string description, string profilePicture, List<string> membersIds)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync()) 
            {
                var members = new List<Member>();
                foreach (string memberId in membersIds)
                {
                    var member = await dbContext.Members.FirstOrDefaultAsync(m => m.Id == memberId);
                    if (member == null)
                    {
                        throw new NotFoundException(typeof(Member).ToString());
                    }
                    members.Add(member);
                }

                var group = new Group();
                group.Name = name;
                group.Description = description;
                group.ProfilePicture = profilePicture;
                group.Members = members;
                await dbContext.Groups.AddAsync(group);
                await dbContext.SaveChangesAsync();
                return group;
            }
        }

        public async Task<bool> Delete(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == id);
                if (group == null)
                {
                    throw new NotFoundException(typeof(Group).ToString());
                }
                dbContext.Groups.Remove(group);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<Group> Get(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == id);
                if (group == null)
                {
                    throw new NotFoundException(typeof(Group).ToString());
                }
                return group;
            }
        }

        public async Task<GroupRequest> ReviewRequest(string requestId, string note, bool isApproved)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var request = await dbContext.GroupRequests.FirstOrDefaultAsync(g => g.Id == requestId);
                if (request == null)
                {
                    throw new NotFoundException(typeof(GroupRequest).ToString());
                }
                request.ReviewNote = note;
                request.IsReviewed = true;
                request.IsApproved = isApproved;
                await dbContext.SaveChangesAsync();
                return request;
            }
        }

        public async Task<GroupRequest> SubmitRequest(string groupId, string note)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
                if (group == null)
                {
                    throw new NotFoundException(typeof(Group).ToString());
                }

                var request = new GroupRequest();
                request.Group = group;
                request.SubmitNote = note;
                request.IsApproved = false;
                request.IsReviewed = false;
                await dbContext.GroupRequests.AddAsync(request);
                return request;
            }
        }

        public async Task<Group> Update(string id, string? name = null, string? description = null, string? profilePicture = null, 
            List<string>? membersIds = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == id);
                if (group == null)
                {
                    throw new NotFoundException(typeof(Group).ToString());
                }

                var newName = name == null ? group.Name : name;
                var newDescription = description == null ? group.Description : description;
                var newProfilePicture = profilePicture == null ? group.ProfilePicture : profilePicture;
                var newMembers = new List<Member>();
                if (membersIds != null)
                {
                    foreach (var memberId in membersIds)
                    {
                        var member = await dbContext.Members.FirstOrDefaultAsync(m => m.Id == memberId);
                        if (member == null)
                        {
                            throw new NotFoundException(typeof(Member).ToString());
                        }
                        newMembers.Add(member);
                    }
                }
                else
                {
                    newMembers = group.Members;
                }

                group.Name = newName;
                group.Description = newDescription;
                group.ProfilePicture = newProfilePicture;
                group.Members = newMembers;
                await dbContext.SaveChangesAsync();
                return group;
            }
        }

        public async Task<List<Comment>> GetComments(string id, int pageNumber = 1, int pageSize = Constants.CommentsPerPage)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var comments = await dbContext.Comments.Where(c => c.Title.Id == id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                return comments;
            }
        }

        public async Task<Comment> AddComment(string groupId, string content)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var commnentUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == Functions.GetCurrentUserId());
                if (!commnentUser.EmailConfirmed)
                {
                    throw new EmailNotConfirmedException();
                }

                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
                if (group == null)
                {
                    throw new NotFoundException(typeof(Group).ToString());
                }

                if (string.IsNullOrEmpty(content))
                {
                    throw new ArgumentException(nameof(content));
                }

                var comment = new Comment();
                comment.Group = group;
                comment.Content = content;
                await dbContext.Comments.AddAsync(comment);
                return comment;
            }
        }

        public async Task<Comment> UpdateComment(string commentId, string? content = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                if (content == "")
                {
                    throw new ArgumentException(nameof(content));
                }

                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
                if (comment == null)
                {
                    throw new NotFoundException(typeof(Comment).ToString());
                }

                comment.Content = content != null ? content : comment.Content;
                await dbContext.Comments.AddAsync(comment);
                return comment;
            }
        }

        public async Task<bool> DeleteComment(string commentId)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
                if (comment == null)
                {
                    throw new NotFoundException(typeof(Comment).ToString());
                }

                dbContext.Comments.Remove(comment);
                return true;
            }
        }
    }
}
