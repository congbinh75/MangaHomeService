using MangaHomeService.Models;
using MangaHomeService.Utils;
using static MangaHomeService.Utils.Enums;

namespace MangaHomeService.Services.Interfaces
{
    public interface ITitleService
    {
        Task<Title> Get(string id);
        Task<List<Title>> Search(string keyword, int pageNumber = 1, int pageSize = Constants.TitlesPerPage);
        Task<List<Title>> AdvancedSearch(string name = "", string author = "", string artist = "", List<string>? genreIds = null,
            List<string>? themeIds = null, string originalLanguageId = "", List<string>? languageIds = null, List<int>? statuses = null,
            bool sortByLastest = false, bool sortByHottest = false, int pageNumber = 1, int pageSize = Constants.TitlesPerPage);
        Task<Title> Add(string name, string description = "", string artwork = "", string author = "", string artist = "",
            Enums.TitleStatus status = Enums.TitleStatus.NotYetReleased, double rating = 0, int ratingVotes = 0, int views = 0,
            int bookmarks = 0, List<TitleOtherName>? otherNames = null, Language? originalLanguage = null, List<Genre>? genres = null,
            List<Theme>? themes = null, List<Chapter>? chapters = null, List<Comment>? comments = null, bool isApproved = false);
        Task<Title> Update(string id, string name = "", string description = "", string artwork = "", string author = "", string artist = "", 
            Enums.TitleStatus? status = null, double rating = -1, int ratingVotes = -1, int views = -1, int bookmarks = -1, 
            List<TitleOtherName>? otherNames = null, string originalLanguageId = "", List<Genre>? genres = null, List<Theme>? themes = null, 
            List<Chapter>? chapters = null, List<Comment>? comments = null, bool? isApproved = null);
        Task<bool> Delete(string id);
        Task<TitleRequest> Submit(string titleId);
        Task<TitleRequest> ApproveOrRejectRequest(string requestId, bool isApproved);
    }
}
