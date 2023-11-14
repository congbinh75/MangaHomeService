using MangaHomeService.Models;
using MangaHomeService.Utils;
using static MangaHomeService.Utils.Enums;

namespace MangaHomeService.Services.Interfaces
{
    public interface ITitleService
    {
        public Task<Title> Get(string id);
        public Task<List<Title>> Search(string keyword, int pageNumber = 1, int pageSize = Constants.TitlesPerPage);
        public Task<List<Title>> AdvancedSearch(string name = "", string author = "", string artist = "", List<string>? genreIds = null,
            List<string>? themeIds = null, List<string>? demographicsIds = null, string originalLanguageId = "", List<string>? languageIds = null, 
            List<int>? statuses = null, bool sortByLastest = false, bool sortByHottest = false, int pageNumber = 1, int pageSize = Constants.TitlesPerPage);
        public Task<Title> Add(string name, string description = "", string artwork = "", string authorId = "", string artistId = "",
            TitleStatus status = TitleStatus.NotYetReleased, double rating = 0, int ratingVotes = 0, int views = 0, int bookmarks = 0,
            List<string>? otherNamesIds = null, string? originalLanguageId = null, List<string>? genresIds = null, List<string>? themesIds = null,
            List<string>? demographicsIds = null, List<string>? chaptersIds = null, List<string>? commentsIds = null, bool isApproved = false);
        public Task<Title> Update(string id, string name = "", string description = "", string artwork = "", string authorId = "", string artistId = "", 
            TitleStatus? status = null, double rating = -1, int ratingVotes = -1, int views = -1, int bookmarks = -1, 
            List<string>? otherNamesIds = null, string originalLanguageId = "", List<string>? genresIds = null, List<string>? themesIds = null,
            List<string>? demographicsIds = null, List<string>? chaptersIds = null, List<string>? commentsIds = null, bool? isApproved = null);
        public Task<bool> Delete(string id);
        public Task<TitleRequest> SubmitRequest(string titleId, string groupId, string note);
        public Task<TitleRequest> ReviewRequest(string requestId, bool isApproved, string note);
        public Task<Title> AddRating(string id, int rating, string? userId = null);
        public Task<Title> RemoveRating(string id, string? userId = null);
    }
}
