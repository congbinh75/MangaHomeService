using MangaHomeService.Models;
using MangaHomeService.Utils;

namespace MangaHomeService.Services.Interfaces
{
    public interface ITitleService
    {
        Task<Title> Get(string id);
        Task<List<Title>> Search(string keyword, int count, int page);
        Task<List<Title>> Search(string name = "", string author = "", string artist = "", List<string>? genreIds = null,
            List<string>? themeIds = null, string originalLanguageId = "", List<string>? languageIds = null, List<int>? statuses = null,
            bool sortByLastest = false, bool sortByHottest = false, int count = Constants.TitlesPerPage, int page = 1);
    }
}
