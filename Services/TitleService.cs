using MangaHomeService.Models;
using MangaHomeService.Models.Entities;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;
using static MangaHomeService.Utils.Enums;

namespace MangaHomeService.Services
{
    public class TitleService(IDbContextFactory<MangaHomeDbContext> contextFactory,
        ITokenInfoProvider tokenInfoProvider,
        IConfiguration configuration) : ITitleService
    {
        public async Task<Title> Get(string id)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var title = await dbContext.Titles.Where(x => x.Id == id).Include(x => x.Chapters).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(Title));
            return title;
        }

        public async Task<ICollection<Title>> Search(string keyword, int pageNumber = 1, int pageSize = Constants.TitlesPerPage)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
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
            using var dbContext = await contextFactory.CreateDbContextAsync();
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
                var gernes = await dbContext.Tags.OfType<Gerne>().Where(x => genreIds.Contains(x.Id)).ToListAsync();
                titles = titles.Where(x => gernes.All(y => x.Gernes.Contains(y))).ToList();
            }

            if (themeIds != null && themeIds.Count > 0)
            {
                var themes = await dbContext.Tags.OfType<Theme>().Where(x => themeIds.Contains(x.Id)).ToListAsync();
                titles = titles.Where(x => themes.All(y => x.Themes?.Contains(y) ?? false)).ToList();
            }

            if (demographicsIds != null && demographicsIds.Count > 0)
            {
                var demographics = await dbContext.Tags.OfType<Demographic>().Where(x => demographicsIds.Contains(x.Id)).ToListAsync();
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
            ICollection<string>? artistsIds = null, TitleStatus status = TitleStatus.NotYetReleased, ICollection<string>? otherNamesIds = null,
            string? originalLanguageId = null, ICollection<string>? genresIds = null, ICollection<string>? themesIds = null, ICollection<string>? demographicsIds = null)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();

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

            var genres = new List<Gerne>();
            if (genresIds != null)
            {
                genres = await dbContext.Tags.OfType<Gerne>().Where(g => genresIds.Contains(g.Id)).ToListAsync();
                if (genres.Count != genresIds.Count)
                {
                    throw new NotFoundException(nameof(Tag) + ":" + nameof(Title.Gernes));
                }
            }

            var themes = new List<Theme>();
            if (themesIds != null)
            {
                themes = await dbContext.Tags.OfType<Theme>().Where(t => themesIds.Contains(t.Id)).ToListAsync();
                if (themes.Count != themesIds.Count)
                {
                    throw new NotFoundException(nameof(Tag) + ":" + nameof(Title.Themes));
                }
            }

            var demographics = new List<Demographic>();
            if (demographicsIds != null)
            {
                demographics = await dbContext.Tags.OfType<Demographic>().Where(d => demographicsIds.Contains(d.Id)).ToListAsync();
                if (demographics.Count != demographicsIds.Count)
                {
                    throw new NotFoundException(nameof(Tag) + ":" + nameof(Title.Demographics));
                }
            }

            var title = new Title
            {
                Name = name,
                Description = description ?? "",
                Artwork = artwork == null ? "" :
                    await Functions.UploadFileAsync(artwork, configuration["FilesStoragePath.TitlesImagesPath"]),
                Authors = authors,
                Artists = artists,
                Status = status,
                Rating = 0,
                RatingVotes = 0,
                Views = 0,
                OtherNames = otherNames,
                Bookmarks = 0,
                OriginalLanguage = originalLanguageId != null ?
                    await dbContext.Languages.FirstOrDefaultAsync(a => a.Id == originalLanguageId) ??
                    throw new NotFoundException(nameof(Language)) : null,
                Gernes = genres,
                Themes = themes,
                Chapters = new List<Chapter>(),
                Comments = new List<Comment>(),
                IsApproved = false,
                Demographics = demographics,
                TitleRatings = new List<TitleRating>()
            };

            await dbContext.Titles.AddAsync(title);
            await dbContext.SaveChangesAsync();
            return title;
        }

        public async Task<Title> Update(string id, string? name = "", string? description = null, IFormFile? artwork = null,
            ICollection<string>? authorsIds = null, ICollection<string>? artistsIds = null, TitleStatus? status = null,
            double rating = 0, int ratingVotes = 0, int views = 0, int bookmarks = 0, ICollection<string>? otherNamesIds = null,
            string? originalLanguageId = null, ICollection<string>? genresIds = null, ICollection<string>? themesIds = null,
            ICollection<string>? demographicsIds = null, ICollection<string>? chaptersIds = null, ICollection<string>? commentsIds = null,
            bool? isApproved = null)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
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

            var genres = new List<Gerne>();
            if (genresIds != null)
            {
                genres = await dbContext.Tags.OfType<Gerne>().Where(g => genresIds.Contains(g.Id)).ToListAsync();
                if (genres.Count != genresIds.Count)
                {
                    throw new NotFoundException(nameof(Tag) + ":" + nameof(Title.Gernes));
                }
            }

            var themes = new List<Theme>();
            if (themesIds != null)
            {
                themes = await dbContext.Tags.OfType<Theme>().Where(t => themesIds.Contains(t.Id)).ToListAsync();
                if (themes.Count != themesIds.Count)
                {
                    throw new NotFoundException(nameof(Tag) + ":" + nameof(Title.Themes));
                }
            }

            var demographics = new List<Demographic>();
            if (demographicsIds != null)
            {
                demographics = await dbContext.Tags.OfType<Demographic>().Where(d => demographicsIds.Contains(d.Id)).ToListAsync();
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
                await Functions.UploadFileAsync(artwork, configuration["FilesStoragePath.TitlesImagesPath"]);
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
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var title = await dbContext.Titles.Where(t => t.Id == id).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(Title));
            dbContext.Titles.Remove(title);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Title> AddRating(string id, int ratingValue, string? userId = null)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();

            var ratingUserId = userId ?? tokenInfoProvider.Id;
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
                User = user,
                TitleId = title.Id,
                UserId = user.Id
            };
            await dbContext.TitleRatings.AddAsync(rating);

            int ratingCount = await dbContext.TitleRatings.Where(t => t.Title.Id == id).CountAsync();
            int ratingSum = await dbContext.TitleRatings.Where(t => t.Title.Id == id).SumAsync(t => t.Rating);
            title.Rating = ratingSum / ratingCount;
            title.RatingVotes = ratingCount;
            await dbContext.SaveChangesAsync();
            return title;
        }

        public async Task<Title> UpdateRating(string id, int rating, string? userId = null)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();

            var ratingUserId = userId ?? tokenInfoProvider.Id;
            var existingRating = await dbContext.TitleRatings.FirstOrDefaultAsync(t => t.Title.Id == id && t.User.Id == ratingUserId)
                ?? throw new NotFoundException(nameof(TitleRating));
            var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == id) ??
                throw new NotFoundException(nameof(Title));
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == ratingUserId) ??
                throw new NotFoundException(nameof(User));
            existingRating.Rating = rating;
            await dbContext.SaveChangesAsync();

            int ratingCount = await dbContext.TitleRatings.Where(t => t.Title.Id == id).CountAsync();
            int ratingSum = await dbContext.TitleRatings.Where(t => t.Title.Id == id).SumAsync(t => t.Rating);
            title.Rating = ratingSum / ratingCount;
            title.RatingVotes = ratingCount;
            await dbContext.SaveChangesAsync();
            return title;
        }

        public async Task<Title> RemoveRating(string id, string? userId = null)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var ratingUserId = userId == null ? tokenInfoProvider.Id : userId;
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

        public async Task<bool> RegisterView(string id)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == id) ?? throw new NotFoundException(nameof(Title));
            var currentTime = DateTime.UtcNow;

            var titlesViewCountsWeek = await dbContext.ViewsCounts.FirstOrDefaultAsync(v => v.Title.Id == id && v.Date == currentTime.Date);
            if (titlesViewCountsWeek != null)
            {
                titlesViewCountsWeek.Views += 1;
            }
            else
            {
                var viewsCount = new ViewsCount
                {
                    Title = title,
                    Views = 1,
                    Date = currentTime.Date,
                };
                await dbContext.ViewsCounts.AddAsync(viewsCount);
            }
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
