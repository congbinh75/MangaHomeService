using MangaHomeService.Models;

namespace MangaHomeService.Services.Interfaces
{
    public interface ITitleService
    {
        Task<Title> Get(string id);
        Task<List<Title>> Search(string keyword, int count, int page);
        Task<List<Title>> Search(string name, List<string> authors, List<string> artists, List<string> genreIds, List<Theme> themeIds, 
            string originalLanguageId, List<string> languageIds, List<int> statuses);
    }
}
