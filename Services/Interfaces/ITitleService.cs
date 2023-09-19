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
        Task Add(string name, string description = "", string artwork = "", string author = "", string artist = "", TitleStatus status = 0, 
            double rating = 0, int ratingVotes = 0, int views = 0, int bookmarks = 0, List<TitleOtherName> otherNames = null, 
            Language originalLanguage = null, List<Genre> genres = null, List<Theme> themes = null, List<Chapter> chapters = null, 
            List<Comment> comments = null);
        Task Update(string id, string name = "", string description = "", string artwork = "", string author = "", string artist = "", 
            TitleStatus status = 0, double rating = 0, int ratingVotes = 0, int views = 0, int bookmarks = 0, 
            List<TitleOtherName> otherNames = null, Language originalLanguage = null, List<Genre> genres = null, List<Theme> themes = null, 
            List<Chapter> chapters = null, List<Comment> comments = null);
        Task Delete(string id);
    }
}
