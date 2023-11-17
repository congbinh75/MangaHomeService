using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;
using static MangaHomeService.Utils.Enums;

namespace MangaHomeService.Services
{
    public interface ITitleService
    {
        public Task<Title> Get(string id);
        public Task<List<Title>> Search(string keyword, int pageNumber = 1, int pageSize = Constants.TitlesPerPage);
        public Task<List<Title>> AdvancedSearch(string name = "", string author = "", string artist = "", List<string>? genreIds = null,
            List<string>? themeIds = null, List<string>? demographicsIds = null, string originalLanguageId = "", List<string>? languageIds = null,
            List<int>? statuses = null, bool sortByLastest = false, bool sortByHottest = false, int pageNumber = 1, int pageSize = Constants.TitlesPerPage);
        public Task<Title> Add(string name, string description = "", string artwork = "", string authorId = "", string artistId = "",
            TitleStatus status = TitleStatus.NotYetReleased, double rating = 0, int ratingVotes = 0, int views = 0, int bookmarks = 0,
            List<string>? otherNamesIds = null, string? originalLanguageId = null, List<string>? genresIds = null, List<string>? themesIds = null,
            List<string>? demographicsIds = null, List<string>? chaptersIds = null, List<string>? commentsIds = null, bool isApproved = false);
        public Task<Title> Update(string id, string name = "", string description = "", string artwork = "", string authorId = "", string artistId = "",
            TitleStatus? status = null, double rating = -1, int ratingVotes = -1, int views = -1, int bookmarks = -1,
            List<string>? otherNamesIds = null, string originalLanguageId = "", List<string>? genresIds = null, List<string>? themesIds = null,
            List<string>? demographicsIds = null, List<string>? chaptersIds = null, List<string>? commentsIds = null, bool? isApproved = null);
        public Task<bool> Delete(string id);
        public Task<TitleRequest> SubmitRequest(string titleId, string groupId, string note);
        public Task<TitleRequest> ReviewRequest(string requestId, bool isApproved, string note);
        public Task<Title> AddRating(string id, int rating, string? userId = null);
        public Task<Title> RemoveRating(string id, string? userId = null);
    }

    public class TitleService : ITitleService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;
        private readonly ITokenInfoProvider _tokenInfoProvider;

        public TitleService(IDbContextFactory<MangaHomeDbContext> contextFactory, ITokenInfoProvider tokenInfoProvider)
        {
            _contextFactory = contextFactory;
            _tokenInfoProvider = tokenInfoProvider;
        }

        public async Task<Title> Get(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var title = await dbContext.Titles.Where(x => x.Id == id).Include(x => x.Chapters).FirstOrDefaultAsync();
                if (title == null) 
                {
                    throw new NotFoundException(typeof(Title).ToString());
                }
                return title;
            }
        }

        public async Task<List<Title>> Search(string keyword, int pageNumber = 1, int pageSize = Constants.TitlesPerPage)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var titles = await dbContext.Titles.Where(x => x.Name.Contains(keyword.Trim()) 
                || x.OtherNames.Any(y => y.Name.Contains(keyword.Trim())) 
                || x.Author.Name.Contains(keyword.Trim())
                || x.Artist.Name.Contains(keyword.Trim())).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                return titles;
            } 
        }

        public async Task<List<Title>> AdvancedSearch(string name = "", string author = "", string artist = "", List<string>? genreIds = null, 
            List<string>? themeIds = null, List<string>? demographicsIds = null, string originalLanguageId = "", List<string>? languageIds = null, 
            List<int>? statuses = null, bool sortByLastest = false, bool sortByHottest = false, int pageNumber = 1, int pageSize = Constants.TitlesPerPage)
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
                                                x.OtherNames.Any(y => y.Name.Contains(name))).ToList();
                }

                if (!string.IsNullOrEmpty(author))
                {
                    titles = titles.Where(x => x.Author.Name.Contains(author)).ToList();
                }

                if (!string.IsNullOrEmpty(artist))
                {
                    titles = titles.Where(x => x.Artist.Name.Contains(artist)).ToList();
                }

                if(genreIds != null && genreIds.Count > 0)
                {
                    var gernes = await dbContext.Tags.Where(x => genreIds.Contains(x.Id) && x.Type == (int)TagType.Gerne).ToListAsync();
                    titles = titles.Where(x => gernes.All(y => x.Gernes.Contains(y))).ToList();
                }

                if (themeIds != null && themeIds.Count > 0)
                {
                    var themes = await dbContext.Tags.Where(x => themeIds.Contains(x.Id) && x.Type == (int)TagType.Theme).ToListAsync();
                    titles = titles.Where(x => themes.All(y => x.Themes.Contains(y))).ToList();
                }

                if (demographicsIds != null && demographicsIds.Count > 0)
                {
                    var demographics = await dbContext.Tags.Where(x => demographicsIds.Contains(x.Id) && x.Type == (int)TagType.Demographic).ToListAsync();
                    titles = titles.Where(x => demographics.All(y => x.Demographics.Contains(y))).ToList();
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

        public async Task<Title> Add(string name, string description = "", string artwork = "", string authorId = "", string artistId = "",
            TitleStatus status = TitleStatus.NotYetReleased, double rating = 0, int ratingVotes = 0, int views = 0, int bookmarks = 0, 
            List<string>? otherNamesIds = null, string? originalLanguageId = null, List<string>? genresIds = null, List<string>? themesIds = null, 
            List<string>? demographicsIds = null, List<string>? chaptersIds = null, List<string>? commentsIds = null, bool isApproved = false)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var author = !string.IsNullOrEmpty(authorId) ? await dbContext.People.FirstOrDefaultAsync(a => a.Id == authorId) : null;
                if (author == null)
                {
                    throw new NotFoundException(typeof(Person).ToString());
                }

                var artist = !string.IsNullOrEmpty(artistId) ? await dbContext.People.FirstOrDefaultAsync(a => a.Id == artistId) : null;
                if (artist == null)
                {
                    throw new NotFoundException(typeof(Person).ToString());
                }

                var originalLanguage = !string.IsNullOrEmpty(artistId) ? 
                    await dbContext.Languages.FirstOrDefaultAsync(a => a.Id == originalLanguageId) : null;
                if (originalLanguage == null)
                {
                    throw new NotFoundException(typeof(Language).ToString());
                }

                var otherNames = new List<OtherName>();
                if (otherNamesIds != null)
                {
                    foreach (var otherNameId in otherNamesIds)
                    {
                        var otherName = await dbContext.OtherNames.FirstOrDefaultAsync(t => t.Id == otherNameId);
                        if (otherName == null) 
                        {
                            throw new NotFoundException(typeof(OtherName).ToString());
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
                            throw new NotFoundException(typeof(Tag).ToString() + TagType.Gerne);
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
                            throw new NotFoundException(typeof(Tag).ToString() + TagType.Theme);
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
                            throw new NotFoundException(typeof(Chapter).ToString());
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
                            throw new NotFoundException(typeof(Comment).ToString());
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
                            throw new NotFoundException(typeof(Tag).ToString() + TagType.Demographic);
                        }
                        demographics.Add(demographic);
                    }
                }

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
                title.OtherNames = otherNames;
                title.Bookmarks = bookmarks;
                title.OtherNames = otherNames;
                title.OriginalLanguage = originalLanguage;
                title.Gernes = genres;
                title.Themes = themes;
                title.Chapters = chapters;
                title.Comments = comments;
                title.IsAprroved = isApproved;
                title.Demographics = demographics;

                await dbContext.Titles.AddAsync(title);
                await dbContext.SaveChangesAsync();
                return title;
            }
        }

        public async Task<Title> Update(string id, string name = "", string description = "", string artwork = "", string authorId = "", string artistId = "",
            Enums.TitleStatus? status = null, double rating = -1, int ratingVotes = -1, int views = -1, int bookmarks = -1,
            List<string>? otherNamesIds = null, string originalLanguageId = "", List<string>? genresIds = null, List<string>? themesIds = null,
            List<string>? demographicsIds = null, List<string>? chaptersIds = null, List<string>? commentsIds = null, bool? isApproved = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync()) 
            {
                var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == id);
                if (title == null) 
                {
                    throw new NotFoundException(typeof(Title).ToString());
                }

                var author = !string.IsNullOrEmpty(authorId) ? await dbContext.People.FirstOrDefaultAsync(a => a.Id == authorId) : null;
                if (author == null)
                {
                    throw new NotFoundException(typeof(Person).ToString());
                }

                var artist = !string.IsNullOrEmpty(artistId) ? await dbContext.People.FirstOrDefaultAsync(a => a.Id == artistId) : null;
                if (artist == null)
                {
                    throw new NotFoundException(typeof(Person).ToString());
                }

                var originalLanguage = !string.IsNullOrEmpty(artistId) ?
                    await dbContext.Languages.FirstOrDefaultAsync(a => a.Id == originalLanguageId) : null;
                if (originalLanguage == null)
                {
                    throw new NotFoundException(typeof(Language).ToString());
                }

                var otherNames = new List<OtherName>();
                if (otherNamesIds != null)
                {
                    foreach (var otherNameId in otherNamesIds)
                    {
                        var otherName = await dbContext.OtherNames.FirstOrDefaultAsync(t => t.Id == otherNameId);
                        if (otherName == null)
                        {
                            throw new NotFoundException(typeof(OtherName).ToString());
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
                            throw new NotFoundException(typeof(Tag).ToString() + TagType.Gerne);
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
                            throw new NotFoundException(typeof(Tag).ToString() + TagType.Theme);
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
                            throw new NotFoundException(typeof(Chapter).ToString());
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
                            throw new NotFoundException(typeof(Comment).ToString());
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
                            throw new NotFoundException(typeof(Tag).ToString() + TagType.Demographic);
                        }
                        demographics.Add(demographic);
                    }
                }

                title.Name = !string.IsNullOrEmpty(name) ? name : title.Name;
                title.Description = !string.IsNullOrEmpty(description) ? description : title.Description;
                title.Artwork = !string.IsNullOrEmpty(artwork) ? artwork : title.Artwork;
                title.Artist = artist;
                title.Author = author;
                title.OtherNames = otherNames;
                title.Gernes = genres;
                title.Themes = themes;
                title.Chapters = chapters;
                title.Comments = comments;
                title.Status = status != null ? (Enums.TitleStatus)status : title.Status;
                title.IsAprroved = isApproved != null ? (bool)isApproved : title.IsAprroved;
                title.OriginalLanguage = originalLanguage;
                title.Demographics = demographics;

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
                    throw new NotFoundException(typeof(Title).ToString());
                }
                dbContext.Titles.Remove(title);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<TitleRequest> SubmitRequest(string titleId, string groupId, string note)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var title = await dbContext.Titles.Where(t => t.Id == titleId && t.IsAprroved == false).FirstOrDefaultAsync();
                if (title == null)
                {
                    throw new NotFoundException(typeof(Title).ToString());
                }

                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
                if (group == null)
                {
                    throw new NotFoundException(typeof(Group).ToString());
                }

                var request = new TitleRequest();
                request.Title = title;
                request.Group = group;
                request.SubmitNote = note;

                await dbContext.TitleRequests.AddAsync(request);
                await dbContext.SaveChangesAsync();
                return request;
            }
        }

        public async Task<TitleRequest> ReviewRequest(string requestId, bool isApproved, string note)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var request = await dbContext.TitleRequests.Where(r => r.Id == requestId).Include(r => r.Title).FirstOrDefaultAsync();
                if (request == null)
                {
                    throw new NotFoundException(typeof(TitleRequest).ToString());
                }
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
        }

        public async Task<Title> AddRating(string id, int ratingValue, string? userId = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                if (ratingValue < 1 && ratingValue > 5)
                {
                    throw new Exception();
                }

                var ratingUserId = userId == null ? _tokenInfoProvider.Id : userId;
                var existingRating = await dbContext.TitleRatings.Where(t => t.Title.Id == id && t.User.Id == ratingUserId).FirstOrDefaultAsync();
                if (existingRating != null)
                {
                    throw new Exception();
                }

                var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == id);
                if (title == null)
                {
                    throw new NotFoundException(typeof(Title).ToString());
                }

                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == ratingUserId);
                if (user == null)
                {
                    throw new NotFoundException(typeof(User).ToString());
                }

                var rating = new TitleRating();
                rating.Title = title;
                rating.Rating = ratingValue;
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
        }

        public async Task<Title> RemoveRating(string id, string? userId = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var ratingUserId = userId == null ? _tokenInfoProvider.Id : userId;
                var rating = await dbContext.TitleRatings.Where(t => t.Title.Id == id && t.User.Id == ratingUserId).FirstOrDefaultAsync();
                if (rating == null)
                {
                    throw new NotFoundException(typeof(Title).ToString());
                }

                var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == id);
                if (title == null)
                {
                    throw new NotFoundException(typeof(Title).ToString());
                }

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
}
