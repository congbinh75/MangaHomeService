using MangaHomeService.Models;
using MangaHomeService.Utils;

namespace MangaHomeService.Services.Interfaces
{
    public interface IGroupService
    {
        public Task<Group> Get(string id);
        public Task<Group> Add(string name, string description, string profilePicture, List<string> membersIds);
        public Task<Group> Update(string id, string? name = null, string? description = null, string? profilePicture = null,
            List<string>? membersIds = null);
        public Task<bool> Delete(string id);
        public Task<TitleRequest> SubmitRequest(string groupId);
        public Task<TitleRequest> ReviewRequest(string requestId, bool isApproved);
        public Task<List<Comment>> GetComments(string id, int pageNumber = 1, int pageSize = Constants.CommentsPerPage);
        public Task<Comment> AddComment(string groupId, string content);
        public Task<Comment> UpdateComment(string commentId, string? content = null);
        public Task<bool> DeleteComment(string commentId);
    }
}
