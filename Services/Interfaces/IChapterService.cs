using MangaHomeService.Models;

namespace MangaHomeService.Services.Interfaces
{
    public interface IChapterService
    {
        public Task<Chapter?> Get(string id);
        public Task<List<Chapter>> GetByTitle (string titleId);
        public Task<Chapter> Add(double number, string titleId, string groupId, string? volumeId = null, string? languageId = null,
            List<Page>? pages = null, List<Comment>? comments = null, bool? isApproved = null);
        public Task<Chapter> Update(string id, double number = -1, string? titleId = null, string? groupId = null,
            string? volumeId = null, string? languageId = null, List<Page>? pages = null, List<Comment>? comments = null);
        public Task<bool> Delete(string id);
        public Task<Tuple<Chapter, ChapterRequest>> Submit(double number, string titleId, string groupId, string? volumeId = null,
            string? languageId = null, List<Page>? pages = null, List<Comment>? comments = null, bool? isApproved = null);
        public Task<Tuple<Chapter, ChapterRequest>> ApproveOrRejectRequest(string requestId, bool isApproved);
    }
}
