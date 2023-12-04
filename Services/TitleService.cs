using MangaHomeService.Models;
using MangaHomeService.Models.Entities;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static MangaHomeService.Utils.Enums;
using Group = MangaHomeService.Models.Entities.Group;

namespace MangaHomeService.Services
{
    public interface ITitleService
    {
        public Task<Title> Get(string id);
        public Task<ICollection<Title>> Search(string keyword, int pageNumber = 1, int pageSize = Constants.TitlesPerPage);
        public Task<ICollection<Title>> AdvancedSearch(string? name = null, string? author = null, string? artist = null,
            ICollection<string>? genreIds = null, ICollection<string>? themeIds = null, ICollection<string>? demographicsIds = null,
            string? originalLanguageId = null, ICollection<string>? languageIds = null, ICollection<int>? statuses = null,
            bool sortByLastest = false, bool sortByHottest = false, int pageNumber = 1, int pageSize = Constants.TitlesPerPage);
        public Task<Title> Add(string name, string? description = null, IFormFile? artwork = null, ICollection<string>? authorsIds = null,
            ICollection<string>? artistsIds = null, TitleStatus status = TitleStatus.NotYetReleased, double rating = 0, int ratingVotes = 0,
            int views = 0, int bookmarks = 0, ICollection<string>? otherNamesIds = null, string? originalLanguageId = null,
            ICollection<string>? genresIds = null, ICollection<string>? themesIds = null, ICollection<string>? demographicsIds = null,
            ICollection<string>? chaptersIds = null, ICollection<string>? commentsIds = null, bool isApproved = false);
        public Task<Title> Update(string id, string name = "", string? description = null, IFormFile? artwork = null,
            ICollection<string>? authorsIds = null, ICollection<string>? artistsIds = null, TitleStatus? status = null,
            double rating = 0, int ratingVotes = 0, int views = 0, int bookmarks = 0, ICollection<string>? otherNamesIds = null,
            string? originalLanguageId = null, ICollection<string>? genresIds = null, ICollection<string>? themesIds = null,
            ICollection<string>? demographicsIds = null, ICollection<string>? chaptersIds = null, ICollection<string>? commentsIds = null,
            bool? isApproved = null);
        public Task<bool> Remove(string id);
        public Task<Title> AddRating(string id, int rating, string? userId = null);
        public Task<Title> RemoveRating(string id, string? userId = null);
    }

    public class TitleService : ITitleService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;
        private readonly ITokenInfoProvider _tokenInfoProvider;
        private readonly IConfiguration _configuration;

        public TitleService(IDbContextFactory<MangaHomeDbContext> contextFactory,
            ITokenInfoProvider tokenInfoProvider,
            IConfiguration configuration)
        {
            _contextFactory = contextFactory;
            _tokenInfoProvider = tokenInfoProvider;
            _configuration = configuration;
        }

        public async Task<Title> Get(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var title = await dbContext.Titles.Where(x => x.Id == id).Include(x => x.Chapters).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(Title));
            return title;
        }

        public async Task<ICollection<Title>> Search(string keyword, int pageNumber = 1, int pageSize = Constants.TitlesPerPage)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var titles = await dbContext.Titles
                .Where(t => (t.Name.Contains(keyword))
                    || t.OtherNames.Any(o => o.Name.Contains(keyword))
                    || t.Authors.Any(a => a.Name.Contains(keyword))
                    || t.Artists.Any(r => r.Name.Contains(keyword))
                )
                .Skip((pageNumber - 1) * (int)pageSize)
                .Take((int)pageSize)
                .ToListAsync();
            return titles;
        }

        public async Task<ICollection<Title>> AdvancedSearch(string? name = null, string? author = null, string? artist = null,
            ICollection<string>? genreIds = null, ICollection<string>? themeIds = null, ICollection<string>? demographicsIds = null,
            string? originalLanguageId = null, ICollection<string>? languageIds = null, ICollection<int>? statuses = null,
            bool sortByLastest = false, bool sortByHottest = false, int pageNumber = 1, int pageSize = Constants.TitlesPerPage)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var titles = await dbContext.Titles
                .Include(x => x.OtherNames)
                .Include(x => x.Gernes)
                .Include(x => x.Themes)
                .ToListAsync();

            titles = name == null ? titles : titles.Where(x => (x.Name?.Contains(name) ?? false)
            || (x.OtherNames?.Any(y => y.Name?.Contains(name) ?? false) ?? false)).ToList();
            titles = author == null ? titles : titles.Where(x => x.Authors.Any(a => a.Name.Contains(author))).ToList();
            titles = artist == null ? titles : titles.Where(x => x.Artists.Any(a => a.Name.Contains(artist))).ToList();
            titles = originalLanguageId == null ? titles : titles.Where(x => x.OriginalLanguage?.Id == originalLanguageId).ToList();

            if (genreIds != null && genreIds.Count > 0)
            {
                var gernes = await dbContext.Tags.Where(x => genreIds.Contains(x.Id)).ToListAsync();
                titles = titles.Where(x => gernes.All(y => x.Gernes.Contains(y))).ToList();
            }

            if (themeIds != null && themeIds.Count > 0)
            {
                var themes = await dbContext.Tags.Where(x => themeIds.Contains(x.Id)).ToListAsync();
                titles = titles.Where(x => themes.All(y => x.Themes?.Contains(y) ?? false)).ToList();
            }

            if (demographicsIds != null && demographicsIds.Count > 0)
            {
                var demographics = await dbContext.Tags.Where(x => demographicsIds.Contains(x.Id)).ToListAsync();
                titles = titles.Where(x => demographics.All(y => x.Demographics?.Contains(y) ?? false)).ToList();
            }

            if (statuses != null && statuses.Count > 0)
            {
                titles = titles.Where(x => statuses.Contains((int)x.Status)).ToList();
            }

            if (sortByLastest)
            {
                titles = [.. titles.OrderByDescending(x => x.UpdatedTime)];
            }

            if (sortByHottest)
            {
                titles = [.. titles.OrderByDescending(x => x.Views)];
            }

            titles = titles.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return titles;
        }

        public async Task<Title> Add(string name, string? description = null, IFormFile? artwork = null, ICollection<string>? authorsIds = null,
            ICollection<string>? artistsIds = null, TitleStatus status = TitleStatus.NotYetReleased, double rating = 0, int ratingVotes = 0,
            int views = 0, int bookmarks = 0, ICollection<string>? otherNamesIds = null, string? originalLanguageId = null,
            ICollection<string>? genresIds = null, ICollection<string>? themesIds = null, ICollection<string>? demographicsIds = null,
            ICollection<string>? chaptersIds = null, ICollection<string>? commentsIds = null, bool isApproved = false)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();

            var authors = new List<Person>();
            if (authorsIds != null)
            {
                authors = await dbContext.People.Where(a => authorsIds.Contains(a.Id)).ToListAsync();
                if (authors.Count != authorsIds.Count)
                {
                    throw new NotFoundException(nameof(Person) + " : " + nameof(Title.Authors));
                }
            }

            var artists = new List<Person>();
            if (artistsIds != null)
            {
                artists = await dbContext.People.Where(a => artistsIds.Contains(a.Id)).ToListAsync();
                if (artists.Count != artistsIds.Count)
                {
                    throw new NotFoundException(nameof(Person) + " : " + nameof(Title.Artists));
                }
            }

            var otherNames = new List<OtherName>();
            if (otherNamesIds != null)
            {
                otherNames = await dbContext.OtherNames.Where(o => otherNamesIds.Contains(o.Id)).ToListAsync();
                if (otherNames.Count != otherNamesIds.Count)
                {
                    throw new NotFoundException(nameof(OtherName));
                }
            }

            var genres = new List<Tag>();
            if (genresIds != null)
            {
                genres = await dbContext.Tags.Where(g => genresIds.Contains(g.Id)).ToListAsync();
                if (genres.Count != genresIds.Count)
                {
                    throw new NotFoundException(nameof(Tag) + ":" + nameof(Title.Gernes));
                }
            }

            var themes = new List<Tag>();
            if (themesIds != null)
            {
                themes = await dbContext.Tags.Where(t => themesIds.Contains(t.Id)).ToListAsync();
                if (themes.Count != themesIds.Count)
                {
                    throw new NotFoundException(nameof(Tag) + ":" + nameof(Title.Themes));
                }
            }

            var demographics = new List<Tag>();
            if (demographicsIds != null)
            {
                demographics = await dbContext.Tags.Where(d => demographicsIds.Contains(d.Id)).ToListAsync();
                if (demographics.Count != demographicsIds.Count)
                {
                    throw new NotFoundException(nameof(Tag) + ":" + nameof(Title.Demographics));
                }
            }

            var chapters = new List<Chapter>();
            if (chaptersIds != null)
            {
                chapters = await dbContext.Chapters.Where(c => chaptersIds.Contains(c.Id)).ToListAsync();
                if (chapters.Count != chaptersIds.Count)
                {
                    throw new NotFoundException(nameof(Chapter));
                }
            }


            var comments = new List<Comment>();
            if (commentsIds != null)
            {
                comments = await dbContext.Comments.Where(c => commentsIds.Contains(c.Id)).ToListAsync();
                if (comments.Count != commentsIds.Count)
                {
                    throw new NotFoundException(nameof(Comment));
                }
            }

            var title = new Title
            {
                Name = name,
                Description = description ?? "",
                Artwork = artwork == null ? "" :
                    await Functions.UploadFileAsync(artwork, _configuration["FilesStoragePath.TitlesImagesPath"]),
                Authors = authors,
                Artists = artists,
                Status = status,
                Rating = rating,
                RatingVotes = ratingVotes,
                Views = views,
                OtherNames = otherNames,
                Bookmarks = bookmarks,
                OriginalLanguage = originalLanguageId != null ?
                    await dbContext.Languages.FirstOrDefaultAsync(a => a.Id == originalLanguageId) ??
                    throw new NotFoundException(nameof(Language)) : null,
                Gernes = genres,
                Themes = themes,
                Chapters = chapters,
                Comments = comments,
                IsApproved = isApproved,
                Demographics = demographics
            };

            await dbContext.Titles.AddAsync(title);
            await dbContext.SaveChangesAsync();
            return title;
        }

        public async Task<Title> Update(string id, string name = "", string? description = null, IFormFile? artwork = null,
            ICollection<string>? authorsIds = null, ICollection<string>? artistsIds = null, TitleStatus? status = null,
            double rating = 0, int ratingVotes = 0, int views = 0, int bookmarks = 0, ICollection<string>? otherNamesIds = null,
            string? originalLanguageId = null, ICollection<string>? genresIds = null, ICollection<string>? themesIds = null,
            ICollection<string>? demographicsIds = null, ICollection<string>? chaptersIds = null, ICollection<string>? commentsIds = null,
            bool? isApproved = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == id) ??
                throw new NotFoundException(nameof(Title));

            var authors = new List<Person>();
            if (authorsIds != null)
            {
                authors = await dbContext.People.Where(a => authorsIds.Contains(a.Id)).ToListAsync();
                if (authors.Count != authorsIds.Count)
                {
                    throw new NotFoundException(nameof(Person) + " : " + nameof(Title.Authors));
                }
            }

            var artists = new List<Person>();
            if (artistsIds != null)
            {
                artists = await dbContext.People.Where(a => artistsIds.Contains(a.Id)).ToListAsync();
                if (artists.Count != artistsIds.Count)
                {
                    throw new NotFoundException(nameof(Person) + " : " + nameof(Title.Artists));
                }
            }

            var otherNames = new List<OtherName>();
            if (otherNamesIds != null)
            {
                otherNames = await dbContext.OtherNames.Where(o => otherNamesIds.Contains(o.Id)).ToListAsync();
                if (otherNames.Count != otherNamesIds.Count)
                {
                    throw new NotFoundException(nameof(OtherName));
                }
            }

            var genres = new List<Tag>();
            if (genresIds != null)
            {
                genres = await dbContext.Tags.Where(g => genresIds.Contains(g.Id)).ToListAsync();
                if (genres.Count != genresIds.Count)
                {
                    throw new NotFoundException(nameof(Tag) + ":" + nameof(Title.Gernes));
                }
            }

            var themes = new List<Tag>();
            if (themesIds != null)
            {
                themes = await dbContext.Tags.Where(t => themesIds.Contains(t.Id)).ToListAsync();
                if (themes.Count != themesIds.Count)
                {
                    throw new NotFoundException(nameof(Tag) + ":" + nameof(Title.Themes));
                }
            }

            var demographics = new List<Tag>();
            if (demographicsIds != null)
            {
                demographics = await dbContext.Tags.Where(d => demographicsIds.Contains(d.Id)).ToListAsync();
                if (demographics.Count != demographicsIds.Count)
                {
                    throw new NotFoundException(nameof(Tag) + ":" + nameof(Title.Demographics));
                }
            }

            var chapters = new List<Chapter>();
            if (chaptersIds != null)
            {
                chapters = await dbContext.Chapters.Where(c => chaptersIds.Contains(c.Id)).ToListAsync();
                if (chapters.Count != chaptersIds.Count)
                {
                    throw new NotFoundException(nameof(Chapter));
                }
            }


            var comments = new List<Comment>();
            if (commentsIds != null)
            {
                comments = await dbContext.Comments.Where(c => commentsIds.Contains(c.Id)).ToListAsync();
                if (comments.Count != commentsIds.Count)
                {
                    throw new NotFoundException(nameof(Comment));
                }
            }

            title.Name = !string.IsNullOrEmpty(name) ? name : title.Name;
            title.Description = !string.IsNullOrEmpty(description) ? description : title.Description;
            title.Artwork = artwork == null ? title.Artwork :
                await Functions.UploadFileAsync(artwork, _configuration["FilesStoragePath.TitlesImagesPath"]);
            title.Artists = artists;
            title.Authors = authors;
            title.OtherNames = otherNames;
            title.Gernes = genres;
            title.Themes = themes;
            title.Chapters = chapters;
            title.Comments = comments;
            title.Status = status != null ? (Enums.TitleStatus)status : title.Status;
            title.IsApproved = isApproved != null ? (bool)isApproved : title.IsApproved;
            title.OriginalLanguage = originalLanguageId == null ? await dbContext.Languages.FirstOrDefaultAsync(a => a.Id == originalLanguageId) ??
                throw new NotFoundException(nameof(Language)) : title.OriginalLanguage;
            title.Demographics = demographics;

            await dbContext.SaveChangesAsync();
            return title;
        }

        public async Task<bool> Remove(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var title = await dbContext.Titles.Where(t => t.Id == id).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(Title));
            dbContext.Titles.Remove(title);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<TitleRequest> GetRequest(string requestId)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.OfType<TitleRequest>().Where(r => r.Id == requestId).
                Include(r => r.Title).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(TitleRequest));
            return request;
        }

        public async Task<TitleRequest> SubmitRequest(string titleId, string groupId, string note)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var title = await dbContext.Titles.Where(t => t.Id == titleId && t.IsApproved == false).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(Title));
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId) ??
                throw new NotFoundException(nameof(Group));
            var request = new TitleRequest
            {
                Title = title,
                Group = group,
                SubmitNote = note
            };

            await dbContext.Requests.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<TitleRequest> ReviewRequest(string requestId, bool isApproved, string note)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.OfType<TitleRequest>().Where(r => r.Id == requestId).
                Include(r => r.Title).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(TitleRequest));
            if (request.IsReviewed)
            {
                throw new AlreadyReviewedException();
            }

            request.ReviewNote = note;
            request.IsApproved = isApproved;
            request.Title.IsApproved = isApproved;
            request.IsReviewed = true;

            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<Title> AddRating(string id, int ratingValue, string? userId = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            if (ratingValue < 1 && ratingValue > 5)
            {
                throw new Exception();
            }

            var ratingUserId = userId ?? _tokenInfoProvider.Id;
            var existingRating = await dbContext.TitleRatings.Where(t => t.Title.Id == id && t.User.Id == ratingUserId).FirstOrDefaultAsync();
            if (existingRating != null)
            {
                throw new Exception();
            }

            var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == id) ??
                throw new NotFoundException(nameof(Title));
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == ratingUserId) ??
                throw new NotFoundException(nameof(User));
            var rating = new TitleRating
            {
                Title = title,
                Rating = ratingValue,
                User = user
            };
            await dbContext.TitleRatings.AddAsync(rating);

            int ratingCount = await dbContext.TitleRatings.Where(t => t.Title.Id == id).CountAsync();
            int ratingSum = await dbContext.TitleRatings.Where(t => t.Title.Id == id).SumAsync(t => t.Rating);
            title.Rating = ratingSum / ratingCount;
            title.RatingVotes = ratingCount;
            await dbContext.SaveChangesAsync();
            return title;
        }

        public async Task<Title> RemoveRating(string id, string? userId = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var ratingUserId = userId == null ? _tokenInfoProvider.Id : userId;
            var rating = await dbContext.TitleRatings.Where(t => t.Title.Id == id && t.User.Id == ratingUserId).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(Title));
            var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == id) ?? throw new NotFoundException(nameof(Title));
            dbContext.TitleRatings.Remove(rating);

            int ratingCount = await dbContext.TitleRatings.Where(t => t.Title.Id == id).CountAsync();
            int ratingSum = await dbContext.TitleRatings.Where(t => t.Title.Id == id).SumAsync(t => t.Rating);
            title.Rating = ratingSum / ratingCount;
            title.RatingVotes = ratingCount;
            await dbContext.SaveChangesAsync();
            return title;
        }
    }
}
