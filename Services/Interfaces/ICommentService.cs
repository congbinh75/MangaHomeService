using MangaHomeService.Models.Entities;
using MangaHomeService.Utils;

namespace MangaHomeService.Services
{
    public interface ICommentService
    {
        public Task<ICollection<Comment>> Get(string id, string type, int pageNumber = 1, int pageSize = Constants.CommentsPerPage);
        public Task<Comment> Add(string id, Type type, string content);
        public Task<Comment> Update(string id, string? content = null, int? vote = null);
        public Task<bool> Remove(string id);
        public Task<Comment> AddVote(string id, bool isUpvote, string? userId = null);
        public Task<Comment> RemoveVote(string id, string? userId = null);
    }
}