using MangaHomeService.Models.Entities;
using MangaHomeService.Utils;

namespace MangaHomeService.Services
{
    public interface IChapterService
    {
        public Task<Chapter> Get(string id);
        public Task<ICollection<Chapter>> GetByTitle(string titleId, int pageNumber = 1, int pageSize = Constants.ChaptersPerPage);
        public Task<Chapter> Add(double number, string titleId, string groupId, string? volumeId = null, string? languageId = null,
            ICollection<string>? pagesIds = null, ICollection<string>? commentsIds = null, bool isApproved = false, bool isRemoved = false);
        public Task<Chapter> Update(string id, double? number = null, string? titleId = null, string? groupId = null,
            string? volumeId = null, string? languageId = null, ICollection<string>? pagesIds = null, ICollection<string>? commentsIds = null,
            bool? isApproved = null, bool? isRemoved = false);
        public Task<bool> Remove(string id);
        public Task<bool> AddTracking(string id, string? userId = null);
        public Task<bool> RemoveTracking(string id, string? userId = null);
    }
}