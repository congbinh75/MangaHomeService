using MangaHomeService.Models;

namespace MangaHomeService.Services.Interfaces
{
    public interface IChapterService
    {
        public Task<Chapter?> Get(string id);
        public Task<List<Chapter>> GetByTitle (string titleId);
        public Task<Chapter> Add(double number, Title title, Group group, Volume ? volume = null, Language? language = null, 
            List<Page>? pages = null, List<Comment>? comments = null, bool? isApproved = null);
        public Task<Chapter> Update(string id, double number = -1, Title? title = null, Group? group = null, Volume? volume = null, 
            Language? language = null, List<Page>? pages = null, List<Comment>? comments = null);
        public Task<bool> Delete(string id);
        public Task<Tuple<Chapter, ChapterRequest>> Submit(double number, Title title, Group group, Volume? volume = null, Language? language = null,
            List<Page>? pages = null, List<Comment>? comments = null);
        public Task<Tuple<Chapter, ChapterRequest>> ApproveOrRejectRequest(string requestId, bool isApproved);
    }
}
