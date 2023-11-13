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
        public Task<GroupRequest> SubmitRequest(string groupId, string note);
        public Task<GroupRequest> ReviewRequest(string requestId, string note, bool isApproved);
    }
}
