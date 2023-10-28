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
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
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
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
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
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var titles = await dbContext.Titles
                    .Include(x => x.OtherNames)
                    .Include(x => x.Gernes)
                    .Include(x => x.Themes)
                    .ToListAsync();

                if (!string.IsNullOrEmpty(name))
                {
                    titles = titles.Where(x => x.Name.Contains(name) || 
                                                x.OtherNames.Any(y => y.OtherName.Contains(name))).ToList();
                }

                if (!string.IsNullOrEmpty(author))
                {
                    titles = titles.Where(x => x.Author.Contains(author)).ToList();
                }

                if (!string.IsNullOrEmpty(artist))
                {
                    titles = titles.Where(x => x.Artist.Contains(artist)).ToList();
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

                if (!string.IsNullOrEmpty(originalLanguageId))
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

        public async Task<Title> Add(string name, string description = "", string artwork = "", string author = "", string artist = "", 
            Enums.TitleStatus status = Enums.TitleStatus.NotYetReleased, double rating = 0, int ratingVotes = 0, int views = 0, 
            int bookmarks = 0, List<TitleOtherName>? otherNames = null, Language? originalLanguage = null, List<Genre>? genres = null, 
            List<Theme>? themes = null, List<Chapter>? chapters = null, List<Comment>? comments = null, bool isApproved = false)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var title = new Title();
                title.Name = name;
                title.Description = description;
                title.Artwork = artwork;
                title.Author = author;
                title.Artist = artist;
                title.Status = status;
                title.Rating = rating;
                title.RatingVotes = ratingVotes;
                title.Views = views;
                title.Bookmarks = bookmarks;
                title.OtherNames = otherNames;
                title.OriginalLanguage = originalLanguage;
                title.Gernes = genres;
                title.Themes = themes;
                title.Chapters = chapters;
                title.Comments = comments;
                title.IsAprroved = isApproved;

                await dbContext.Titles.AddAsync(title);
                await dbContext.SaveChangesAsync();
                return title;
            }
        }

        public async Task<Title> Update(string id, string name = "", string description = "", string artwork = "", string author = "", 
            string artist = "", Enums.TitleStatus? status = null, double rating = -1, int ratingVotes = -1, 
            int views = -1, int bookmarks = -1, List<TitleOtherName>? otherNames = null, string originalLanguageId = "", 
            List<Genre>? genres = null, List<Theme>? themes = null, List<Chapter>? chapters = null, List<Comment>? comments = null, 
            bool? isApproved = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync()) 
            {
                var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == id);
                if (title == null) 
                {
                    throw new NullReferenceException(nameof(title));
                }

                title.Name = !string.IsNullOrEmpty(name) ? name : title.Name;
                title.Description = !string.IsNullOrEmpty(description) ? description : title.Description;
                title.Artwork = !string.IsNullOrEmpty(artwork) ? artwork : title.Artwork;
                title.Artist = !string.IsNullOrEmpty(artist) ? artist : title.Artist;
                title.OtherNames = otherNames != null ? otherNames : title.OtherNames;
                title.Gernes = genres != null ? genres : title.Gernes;
                title.Themes = themes != null ? themes : title.Themes;
                title.Chapters = chapters != null ? chapters : title.Chapters;
                title.Comments = comments != null ? comments : title.Comments;
                title.Status = status != null ? (Enums.TitleStatus)status : title.Status;
                title.IsAprroved = isApproved != null ? (bool)isApproved : title.IsAprroved;

                if (!string.IsNullOrEmpty(originalLanguageId))
                {
                    var originalLanguage = await dbContext.Languages.FirstOrDefaultAsync(l => l.Id == id);
                    if (originalLanguage != null)
                    {
                        throw new NullReferenceException(nameof(originalLanguage));
                    }
                    title.OriginalLanguage = originalLanguage;
                }

                await dbContext.SaveChangesAsync();
                return title;
            }
        }

        public async Task<bool> Delete(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var title = await dbContext.Titles.Where(t => t.Id == id).FirstOrDefaultAsync();
                if (title == null) 
                {
                    throw new ArgumentException(nameof(id));
                }
                dbContext.Titles.Remove(title);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<TitleRequest> Submit(string titleId)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var submitUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == Functions.GetCurrentUserId());
                var title = await dbContext.Titles.Where(t => t.Id == titleId && t.IsAprroved == false).FirstOrDefaultAsync();
                if (title == null)
                {
                    throw new NullReferenceException(nameof(titleId));
                }

                var request = new TitleRequest();
                request.Title = title;
                request.User = submitUser;

                await dbContext.TitleRequests.AddAsync(request);
                await dbContext.SaveChangesAsync();
                return request;
            }
        }

        public async Task<TitleRequest> ApproveOrRejectRequest(string requestId, bool isApproved)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var request = await dbContext.TitleRequests.Where(r => r.Id == requestId).Include(r => r.Title).FirstOrDefaultAsync();
                if (request.IsApproved != null || request.Title.IsAprroved != null)
                {
                    throw new ArgumentException();
                }

                request.IsApproved = isApproved;
                request.Title.IsAprroved = isApproved;
                await dbContext.SaveChangesAsync();
                return request;
            }
        }
    }
}
