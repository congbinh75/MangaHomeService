using MangaHomeService.Models;
using MangaHomeService.Utils;

namespace MangaHomeService.Services.Interfaces
{
    public interface ICommentService
    {
        public Task<List<Comment>> Get(string id, string type, int pageNumber = 1, int pageSize = Constants.CommentsPerPage);
        public Task<Comment> Add(string id, string type, string content);
        public Task<Comment> Update(string id, string content, int? vote = null);
        public Task<bool> Delete(string id);
        public Task<Comment> Vote(string id, bool isUpvote);
        public Task<Comment> RemoveVote(string id);
    }
}
