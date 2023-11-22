using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;
using static MangaHomeService.Utils.Enums;
using Group = MangaHomeService.Models.Group;

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
        public Task<Title> Add(string name, string? description = null, IFormFile? artwork = null, string? authorId = null,
            string? artistId = null, TitleStatus status = TitleStatus.NotYetReleased, double rating = 0, int ratingVotes = 0,
            int views = 0, int bookmarks = 0, ICollection<string>? otherNamesIds = null, string? originalLanguageId = null,
            ICollection<string>? genresIds = null, ICollection<string>? themesIds = null, ICollection<string>? demographicsIds = null,
            ICollection<string>? chaptersIds = null, ICollection<string>? commentsIds = null, bool isApproved = false);
        public Task<Title> Update(string id, string name = "", string description = "", IFormFile? artwork = null, string authorId = "",
            string artistId = "", TitleStatus? status = null, double rating = -1, int ratingVotes = -1, int views = -1, int bookmarks = -1,
            ICollection<string>? otherNamesIds = null, string originalLanguageId = "", ICollection<string>? genresIds = null,
            ICollection<string>? themesIds = null, ICollection<string>? demographicsIds = null, ICollection<string>? chaptersIds = null,
            ICollection<string>? commentsIds = null, bool? isApproved = null);
        public Task<bool> Delete(string id);
        public Task<TitleRequest> GetRequest(string id);
        public Task<TitleRequest> SubmitRequest(string titleId, string groupId, string note);
        public Task<TitleRequest> ReviewRequest(string requestId, bool isApproved, string note);
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
                throw new NotFoundException(typeof(Title).Name);
            return title;
        }

        public async Task<ICollection<Title>> Search(string keyword, int pageNumber = 1, int pageSize = Constants.TitlesPerPage)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var titles = await dbContext.Titles
                .Where(x =>
                    (x.Name != null && x.Name.Contains(keyword))
                    || (x.OtherNames != null && x.OtherNames.Any(y => y.Name != null && y.Name.Contains(keyword)))
                    || (x.Author != null && x.Author.Name != null && x.Author.Name.Contains(keyword))
                    || (x.Artist != null && x.Artist.Name != null && x.Artist.Name.Contains(keyword))
                )
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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
            titles = author == null ? titles : titles.Where(x => x.Author?.Name?.Contains(author) ?? false).ToList();
            titles = artist == null ? titles : titles.Where(x => x.Artist?.Name?.Contains(artist) ?? false).ToList();
            titles = originalLanguageId == null ? titles : titles.Where(x => x.OriginalLanguage?.Id == originalLanguageId).ToList();

            if (genreIds != null && genreIds.Count > 0)
            {
                var gernes = await dbContext.Tags.Where(x => (x.Id != null && genreIds.Contains(x.Id)) 
                    && x.Type == (int)TagType.Gerne).ToListAsync();
                titles = titles.Where(x => gernes.All(y => x.Gernes?.Contains(y) ?? false)).ToList();
            }

            if (themeIds != null && themeIds.Count > 0)
            {
                var themes = await dbContext.Tags.Where(x => (x.Id != null && themeIds.Contains(x.Id))
                    && x.Type == (int)TagType.Theme).ToListAsync();
                titles = titles.Where(x => themes.All(y => x.Themes?.Contains(y) ?? false)).ToList();
            }

            if (demographicsIds != null && demographicsIds.Count > 0)
            {
                var demographics = await dbContext.Tags.Where(x => (x.Id != null && demographicsIds.Contains(x.Id)) 
                    && x.Type == (int)TagType.Demographic).ToListAsync();
                titles = titles.Where(x => demographics.All(y => x.Demographics?.Contains(y) ?? false)).ToList();
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

        public async Task<Title> Add(string name, string? description = null, IFormFile? artwork = null, string? authorId = null,
            string? artistId = null, TitleStatus status = TitleStatus.NotYetReleased, double rating = 0, int ratingVotes = 0,
            int views = 0, int bookmarks = 0, ICollection<string>? otherNamesIds = null, string? originalLanguageId = null,
            ICollection<string>? genresIds = null, ICollection<string>? themesIds = null, ICollection<string>? demographicsIds = null,
            ICollection<string>? chaptersIds = null, ICollection<string>? commentsIds = null, bool isApproved = false)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var otherNames = new List<OtherName>();
                if (otherNamesIds != null)
                {
                    foreach (var otherNameId in otherNamesIds)
                    {
                        var otherName = await dbContext.OtherNames.FirstOrDefaultAsync(t => t.Id == otherNameId) ??
                            throw new NotFoundException(typeof(OtherName).Name);
                        otherNames.Add(otherName);
                    }
                }

                var genres = new List<Tag>();
                if (genresIds != null)
                {
                    foreach (var genreId in genresIds)
                    {
                        var genre = await dbContext.Tags.FirstOrDefaultAsync(g => g.Id == genreId) ??
                            throw new NotFoundException(typeof(Tag).Name + TagType.Gerne);
                        genres.Add(genre);
                    }
                }

                var themes = new List<Tag>();
                if (themesIds != null)
                {
                    foreach (var themeId in themesIds)
                    {
                        var theme = await dbContext.Tags.FirstOrDefaultAsync(g => g.Id == themeId) ??
                            throw new NotFoundException(typeof(Tag).Name + TagType.Theme);
                        themes.Add(theme);
                    }
                }

                var chapters = new List<Chapter>();
                if (chaptersIds != null)
                {
                    foreach (var chapterId in chaptersIds)
                    {
                        var chapter = await dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == chapterId) ??
                            throw new NotFoundException(typeof(Chapter).Name);
                        chapters.Add(chapter);
                    }
                }


                var comments = new List<Comment>();
                if (commentsIds != null)
                {
                    foreach (var commentId in commentsIds)
                    {
                        var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId) ??
                            throw new NotFoundException(typeof(Comment).Name);
                        comments.Add(comment);
                    }
                }

                var demographics = new List<Tag>();
                if (demographicsIds != null)
                {
                    foreach (var demographicId in demographicsIds)
                    {
                        var demographic = await dbContext.Tags.FirstOrDefaultAsync(c => c.Id == demographicId) ??
                            throw new NotFoundException(typeof(Tag).Name + TagType.Demographic);
                        demographics.Add(demographic);
                    }
                }

                var title = new Title
                {
                    Name = name,
                    Description = description ?? "",
                    Artwork = artwork == null ? "" :
                    await Functions.UploadFileAsync(artwork, _configuration["FilesStoragePath.TitlesImagesPath"]),
                    Author = authorId != null ?
                        await dbContext.People.FirstOrDefaultAsync(a => a.Id == authorId) ??
                        throw new NotFoundException(typeof(Person).Name) : null,
                    Artist = artistId != null ?
                        await dbContext.People.FirstOrDefaultAsync(a => a.Id == artistId) ??
                        throw new NotFoundException(typeof(Person).Name) : null,
                    Status = status,
                    Rating = rating,
                    RatingVotes = ratingVotes,
                    Views = views,
                    OtherNames = otherNames,
                    Bookmarks = bookmarks,
                    OriginalLanguage = originalLanguageId != null ?
                    await dbContext.Languages.FirstOrDefaultAsync(a => a.Id == originalLanguageId) ??
                    throw new NotFoundException(typeof(Language).Name) : null,
                    Gernes = genres,
                    Themes = themes,
                    Chapters = chapters,
                    Comments = comments,
                    IsAprroved = isApproved,
                    Demographics = demographics
                };

                await dbContext.Titles.AddAsync(title);
                await dbContext.SaveChangesAsync();
                return title;
            }
        }

        public async Task<Title> Update(string id, string name = "", string description = "", IFormFile? artwork = null, string authorId = "",
            string artistId = "", TitleStatus? status = null, double rating = -1, int ratingVotes = -1, int views = -1, int bookmarks = -1,
            ICollection<string>? otherNamesIds = null, string originalLanguageId = "", ICollection<string>? genresIds = null,
            ICollection<string>? themesIds = null, ICollection<string>? demographicsIds = null, ICollection<string>? chaptersIds = null,
            ICollection<string>? commentsIds = null, bool? isApproved = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == id) ??
                    throw new NotFoundException(typeof(Title).Name);

                var otherNames = new List<OtherName>();
                if (otherNamesIds != null)
                {
                    foreach (var otherNameId in otherNamesIds)
                    {
                        var otherName = await dbContext.OtherNames.FirstOrDefaultAsync(t => t.Id == otherNameId);
                        if (otherName == null)
                        {
                            throw new NotFoundException(typeof(OtherName).Name);
                        }
                        otherNames.Add(otherName);
                    }
                }

                var genres = new List<Tag>();
                if (genresIds != null)
                {
                    foreach (var genreId in genresIds)
                    {
                        var genre = await dbContext.Tags.FirstOrDefaultAsync(g => g.Id == genreId);
                        if (genre == null)
                        {
                            throw new NotFoundException(typeof(Tag).Name + TagType.Gerne);
                        }
                        genres.Add(genre);
                    }
                }

                var themes = new List<Tag>();
                if (themesIds != null)
                {
                    foreach (var themeId in themesIds)
                    {
                        var theme = await dbContext.Tags.FirstOrDefaultAsync(g => g.Id == themeId);
                        if (theme == null)
                        {
                            throw new NotFoundException(typeof(Tag).Name + TagType.Theme);
                        }
                        themes.Add(theme);
                    }
                }

                var chapters = new List<Chapter>();
                if (chaptersIds != null)
                {
                    foreach (var chapterId in chaptersIds)
                    {
                        var chapter = await dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == chapterId);
                        if (chapter == null)
                        {
                            throw new NotFoundException(typeof(Chapter).Name);
                        }
                        chapters.Add(chapter);
                    }
                }


                var comments = new List<Comment>();
                if (commentsIds != null)
                {
                    foreach (var commentId in commentsIds)
                    {
                        var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
                        if (comment == null)
                        {
                            throw new NotFoundException(typeof(Comment).Name);
                        }
                        comments.Add(comment);
                    }
                }

                var demographics = new List<Tag>();
                if (demographicsIds != null)
                {
                    foreach (var demographicId in demographicsIds)
                    {
                        var demographic = await dbContext.Tags.FirstOrDefaultAsync(c => c.Id == demographicId);
                        if (demographic == null)
                        {
                            throw new NotFoundException(typeof(Tag).Name + TagType.Demographic);
                        }
                        demographics.Add(demographic);
                    }
                }

                title.Name = !string.IsNullOrEmpty(name) ? name : title.Name;
                title.Description = !string.IsNullOrEmpty(description) ? description : title.Description;
                title.Artwork = artwork == null ? title.Artwork :
                    await Functions.UploadFileAsync(artwork, _configuration["FilesStoragePath.TitlesImagesPath"]);
                title.Artist = artistId == null ? await dbContext.People.FirstOrDefaultAsync(a => a.Id == artistId) ??
                    throw new NotFoundException(typeof(Person).Name) : title.Artist;
                title.Author = authorId == null ? await dbContext.People.FirstOrDefaultAsync(a => a.Id == authorId) ??
                    throw new NotFoundException(typeof(Person).Name) : title.Author;
                title.OtherNames = otherNames;
                title.Gernes = genres;
                title.Themes = themes;
                title.Chapters = chapters;
                title.Comments = comments;
                title.Status = status != null ? (Enums.TitleStatus)status : title.Status;
                title.IsAprroved = isApproved != null ? (bool)isApproved : title.IsAprroved;
                title.OriginalLanguage = originalLanguageId == null ? await dbContext.Languages.FirstOrDefaultAsync(a => a.Id == originalLanguageId) ??
                    throw new NotFoundException(typeof(Language).Name) : title.OriginalLanguage;
                title.Demographics = demographics;

                await dbContext.SaveChangesAsync();
                return title;
            }
        }

        public async Task<bool> Delete(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var title = await dbContext.Titles.Where(t => t.Id == id).FirstOrDefaultAsync() ??
                throw new NotFoundException(typeof(Title).Name);
            dbContext.Titles.Remove(title);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<TitleRequest> GetRequest(string requestId)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.OfType<TitleRequest>().Where(r => r.Id == requestId).
                Include(r => r.Title).FirstOrDefaultAsync() ??
                throw new NotFoundException(typeof(TitleRequest).Name);
            return request;
        }

        public async Task<TitleRequest> SubmitRequest(string titleId, string groupId, string note)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var title = await dbContext.Titles.Where(t => t.Id == titleId && t.IsAprroved == false).FirstOrDefaultAsync() ??
                throw new NotFoundException(typeof(Title).Name);
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId) ??
                throw new NotFoundException(typeof(Group).Name);
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
                throw new NotFoundException(typeof(TitleRequest).Name);
            if (request.IsReviewed)
            {
                throw new AlreadyReviewedException();
            }

            request.ReviewNote = note;
            request.IsApproved = isApproved;
            request.Title.IsAprroved = isApproved;
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
                throw new NotFoundException(typeof(Title).Name);
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == ratingUserId) ??
                throw new NotFoundException(typeof(User).Name);
            var rating = new TitleRating
            {
                Title = title,
                Rating = ratingValue,
                User = user
            };
            await dbContext.TitleRatings.AddAsync(rating);

            var currentTitlesRatings = await dbContext.TitleRatings.Where(t => t.Title.Id == id).ToListAsync();
            int sumrating = 0;
            foreach (var currentTitle in currentTitlesRatings)
            {
                sumrating += currentTitle.Rating;
            }
            title.Rating = sumrating / currentTitlesRatings.Count;
            await dbContext.SaveChangesAsync();
            return title;
        }

        public async Task<Title> RemoveRating(string id, string? userId = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var ratingUserId = userId == null ? _tokenInfoProvider.Id : userId;
            var rating = await dbContext.TitleRatings.Where(t => t.Title.Id == id && t.User.Id == ratingUserId).FirstOrDefaultAsync() ??
                throw new NotFoundException(typeof(Title).Name);
            var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == id) ?? throw new NotFoundException(typeof(Title).Name);
            dbContext.TitleRatings.Remove(rating);

            var currentTitlesRatings = await dbContext.TitleRatings.Where(t => t.Title.Id == id).ToListAsync();
            int sumrating = 0;
            foreach (var currentTitle in currentTitlesRatings)
            {
                sumrating += currentTitle.Rating;
            }
            title.Rating = sumrating / currentTitlesRatings.Count;
            await dbContext.SaveChangesAsync();
            return title;
        }
    }
}
