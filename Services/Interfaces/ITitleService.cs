using MangaHomeService.Models.Entities;
using MangaHomeService.Utils;
using static MangaHomeService.Utils.Enums;

namespace MangaHomeService.Services
{
    public interface ITitleService
    {
        public Task<Title> Get(string id);
        public Task<ICollection<Title>> GetFeatured(int category);
        public Task<ICollection<Title>> Search(string keyword, int pageNumber = 1, int pageSize = Constants.TitlesPerPage);
        public Task<ICollection<Title>> AdvancedSearch(string? name = null, string? author = null, string? artist = null,
            ICollection<string>? genreIds = null, ICollection<string>? themeIds = null, ICollection<string>? demographicsIds = null,
            string? originalLanguageId = null, ICollection<string>? languageIds = null, ICollection<int>? statuses = null,
            bool sortByLastest = false, bool sortByHottest = false, int pageNumber = 1, int pageSize = Constants.TitlesPerPage);
        public Task<Title> Add(string name, string? description = null, IFormFile? artwork = null, ICollection<string>? authorsIds = null,
            ICollection<string>? artistsIds = null, TitleStatus status = TitleStatus.NotYetReleased, ICollection<string>? otherNamesIds = null,
            string? originalLanguageId = null, ICollection<string>? genresIds = null, ICollection<string>? themesIds = null,
            ICollection<string>? demographicsIds = null);
        public Task<Title> Update(string id, string? name = "", string? description = null, IFormFile? artwork = null,
            ICollection<string>? authorsIds = null, ICollection<string>? artistsIds = null, TitleStatus? status = null,
            double rating = 0, int ratingVotes = 0, int views = 0, int bookmarks = 0, ICollection<string>? otherNamesIds = null,
            string? originalLanguageId = null, ICollection<string>? genresIds = null, ICollection<string>? themesIds = null,
            ICollection<string>? demographicsIds = null, ICollection<string>? chaptersIds = null, ICollection<string>? commentsIds = null,
            bool? isApproved = null);
        public Task<bool> Remove(string id);
        public Task<Title> AddRating(string id, int rating, string? userId = null);
        public Task<Title> UpdateRating(string id, int rating, string? userId = null);
        public Task<Title> RemoveRating(string id, string? userId = null);
        public Task<bool> RegisterView(string id);
    }
}