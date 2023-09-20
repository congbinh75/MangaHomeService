using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace MangaHomeService.Services
{
    public class TitleService : ITitleService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public TitleService(IDbContextFactory<MangaHomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Title> Get(string id)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentNullException(nameof(id));
                }
                var title = await dbContext.Titles.Where(x => x.Id == id).Include(x => x.Chapters).FirstOrDefaultAsync();
                if (title == null) 
                {
                    throw new Exception();
                }
                return title;
            }
        }

        public async Task<List<Title>> Search(string keyword, int pageNumber = 1, int pageSize = Constants.TitlesPerPage)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                var titles = await dbContext.Titles.Where(x => x.Name.Contains(keyword.Trim()) 
                || x.OtherNames.Any(y => y.OtherName.Contains(keyword.Trim())) 
                || x.Author.Contains(keyword.Trim())
                || x.Artist.Contains(keyword.Trim())).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                return titles;
            } 
        }

        public async Task<List<Title>> AdvancedSearch(string name = "", string author = "", string artist = "", List<string>? genreIds = null, 
            List<string>? themeIds = null, string originalLanguageId = "", List<string>? languageIds = null, List<int>? statuses = null, 
            bool sortByLastest = false, bool sortByHottest = false, int pageNumber = 1, int pageSize = Constants.TitlesPerPage)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                var titles = await dbContext.Titles
                    .Include(x => x.OtherNames)
                    .Include(x => x.Gernes)
                    .Include(x => x.Themes)
                    .ToListAsync();

                if (!string.IsNullOrEmpty(name.Trim()))
                {
                    titles = titles.Where(x => x.Name.Contains(name.Trim()) || 
                                                x.OtherNames.Any(y => y.OtherName.Contains(name.Trim()))).ToList();
                }

                if (!string.IsNullOrEmpty(author.Trim()))
                {
                    titles = titles.Where(x => x.Author.Contains(author.Trim())).ToList();
                }

                if (!string.IsNullOrEmpty(artist.Trim()))
                {
                    titles = titles.Where(x => x.Artist.Contains(artist.Trim())).ToList();
                }

                if(genreIds != null && genreIds.Count > 0)
                {
                    var gernes = await dbContext.Genres.Where(x => genreIds.Contains(x.Id)).ToListAsync();
                    titles = titles.Where(x => gernes.All(y => x.Gernes.Contains(y))).ToList();
                }

                if (themeIds != null && themeIds.Count > 0)
                {
                    var themes = await dbContext.Themes.Where(x => themeIds.Contains(x.Id)).ToListAsync();
                    titles = titles.Where(x => themes.All(y => x.Themes.Contains(y))).ToList();
                }

                if (!string.IsNullOrEmpty(originalLanguageId.Trim()))
                {
                    titles = titles.Where(x => x.OriginalLanguage.Id == originalLanguageId).ToList();
                }

                if (statuses != null && statuses.Count > 0)
                {
                    titles = titles.Where(x => statuses.Contains((int)x.Status)).ToList();
                }

                if (sortByLastest)
                {
                    titles = titles.OrderByDescending(x => x.UpdatedTime).ToList();
                }

                if (sortByHottest)
                {
                    titles = titles.OrderByDescending(x => x.Views).ToList();
                }

                titles = titles.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return titles;
            }
        }

        public async Task Add(string name, string description = "", string artwork = "", string author = "", string artist = "", 
            Enums.TitleStatus status = Enums.TitleStatus.NotYetReleased, double rating = 0, int ratingVotes = 0, int views = 0, 
            int bookmarks = 0, List<TitleOtherName>? otherNames = null, Language? originalLanguage = null, List<Genre>? genres = null, 
            List<Theme>? themes = null, List<Chapter>? chapters = null, List<Comment>? comments = null)
        {
            throw new NotImplementedException();
        }

        public async Task Update(string id, string name = "", string description = "", string artwork = "", string author = "", 
            string artist = "", Enums.TitleStatus status = Enums.TitleStatus.NotYetReleased, double rating = 0, int ratingVotes = 0, 
            int views = 0, int bookmarks = 0, List<TitleOtherName>? otherNames = null, Language? originalLanguage = null, 
            List<Genre>? genres = null, List<Theme>? themes = null, List<Chapter>? chapters = null, List<Comment>? comments = null)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
