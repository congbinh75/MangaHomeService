using MangaHomeService.Models;
using MangaHomeService.Utils;

namespace MangaHomeService.Services.Interfaces
{
    public interface ITitleService
    {
        Task<Title> Get(string id);
        Task<List<Title>> Search(string keyword, int pageNumber = 1, int pageSize = Constants.TitlesPerPage);
        Task<List<Title>> AdvancedSearch(string name = "", string author = "", string artist = "", List<string>? genreIds = null,
            List<string>? themeIds = null, string originalLanguageId = "", List<string>? languageIds = null, List<int>? statuses = null,
            bool sortByLastest = false, bool sortByHottest = false, int pageNumber = 1, int pageSize = Constants.TitlesPerPage);
    }
}
