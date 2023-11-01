using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using static MangaHomeService.Utils.Enums;

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
                                                x.OtherNames.Any(y => y.OtherName.Contains(name))).ToList();
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
                    var gernes = await dbContext.Genres.Where(x => genreIds.Contains(x.Id)).ToListAsync();
                    titles = titles.Where(x => gernes.All(y => x.Gernes.Contains(y))).ToList();
                }

                if (themeIds != null && themeIds.Count > 0)
                {
                    var themes = await dbContext.Themes.Where(x => themeIds.Contains(x.Id)).ToListAsync();
                    titles = titles.Where(x => themes.All(y => x.Themes.Contains(y))).ToList();
                }

                if (demographicsIds != null && demographicsIds.Count > 0)
                {
                    var demographics = await dbContext.Demographics.Where(x => demographicsIds.Contains(x.Id)).ToListAsync();
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
                var author = !string.IsNullOrEmpty(authorId) ? await dbContext.Authors.FirstOrDefaultAsync(a => a.Id == authorId) : null;
                if (!string.IsNullOrEmpty(authorId) && author == null)
                {
                    throw new NullReferenceException(nameof(author));
                }

                var artist = !string.IsNullOrEmpty(artistId) ? await dbContext.Artists.FirstOrDefaultAsync(a => a.Id == artistId) : null;
                if (!string.IsNullOrEmpty(authorId) && artist == null)
                {
                    throw new NullReferenceException(nameof(artist));
                }

                var originalLanguage = !string.IsNullOrEmpty(artistId) ? 
                    await dbContext.Languages.FirstOrDefaultAsync(a => a.Id == originalLanguageId) : null;
                if (!string.IsNullOrEmpty(originalLanguageId) && originalLanguage == null)
                {
                    throw new NullReferenceException(nameof(originalLanguage));
                }

                var otherNames = new List<TitleOtherName>();
                if (otherNamesIds != null)
                {
                    foreach (var otherNameId in otherNamesIds)
                    {
                        var otherName = await dbContext.TitleOtherNames.FirstOrDefaultAsync(t => t.Id == otherNameId);
                        if (otherName == null) 
                        {
                            throw new NullReferenceException(nameof(otherName));
                        }
                        otherNames.Add(otherName);
                    }
                }

                var genres = new List<Genre>();
                if (genresIds != null)
                {
                    foreach (var genreId in genresIds)
                    {
                        var genre = await dbContext.Genres.FirstOrDefaultAsync(g => g.Id == genreId);
                        if (genre == null)
                        {
                            throw new NullReferenceException(nameof(genre));
                        }
                        genres.Add(genre);
                    }
                }

                var themes = new List<Theme>();
                if (themesIds != null)
                {
                    foreach (var themeId in themesIds)
                    {
                        var theme = await dbContext.Themes.FirstOrDefaultAsync(g => g.Id == themeId);
                        if (theme == null)
                        {
                            throw new NullReferenceException(nameof(theme));
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
                            throw new NullReferenceException(nameof(chapter));
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
                            throw new NullReferenceException(nameof(comment));
                        }
                        comments.Add(comment);
                    }
                }

                var demographics = new List<Demographic>();
                if (demographicsIds != null)
                {
                    foreach (var demographicId in demographicsIds)
                    {
                        var demographic = await dbContext.Demographics.FirstOrDefaultAsync(c => c.Id == demographicId);
                        if (demographic == null)
                        {
                            throw new NullReferenceException(nameof(demographic));
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
                    throw new NullReferenceException(nameof(title));
                }

                var author = !string.IsNullOrEmpty(authorId) ? await dbContext.Authors.FirstOrDefaultAsync(a => a.Id == authorId) : null;
                if (!string.IsNullOrEmpty(authorId) && author == null)
                {
                    throw new NullReferenceException(nameof(author));
                }

                var artist = !string.IsNullOrEmpty(artistId) ? await dbContext.Artists.FirstOrDefaultAsync(a => a.Id == artistId) : null;
                if (!string.IsNullOrEmpty(authorId) && artist == null)
                {
                    throw new NullReferenceException(nameof(artist));
                }

                var originalLanguage = !string.IsNullOrEmpty(artistId) ?
                    await dbContext.Languages.FirstOrDefaultAsync(a => a.Id == originalLanguageId) : null;
                if (!string.IsNullOrEmpty(originalLanguageId) && originalLanguage == null)
                {
                    throw new NullReferenceException(nameof(originalLanguage));
                }

                var otherNames = new List<TitleOtherName>();
                if (otherNamesIds != null)
                {
                    foreach (var otherNameId in otherNamesIds)
                    {
                        var otherName = await dbContext.TitleOtherNames.FirstOrDefaultAsync(t => t.Id == otherNameId);
                        if (otherName == null)
                        {
                            throw new NullReferenceException(nameof(otherName));
                        }
                        otherNames.Add(otherName);
                    }
                }

                var genres = new List<Genre>();
                if (genresIds != null)
                {
                    foreach (var genreId in genresIds)
                    {
                        var genre = await dbContext.Genres.FirstOrDefaultAsync(g => g.Id == genreId);
                        if (genre == null)
                        {
                            throw new NullReferenceException(nameof(genre));
                        }
                        genres.Add(genre);
                    }
                }

                var themes = new List<Theme>();
                if (themesIds != null)
                {
                    foreach (var themeId in themesIds)
                    {
                        var theme = await dbContext.Themes.FirstOrDefaultAsync(g => g.Id == themeId);
                        if (theme == null)
                        {
                            throw new NullReferenceException(nameof(theme));
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
                            throw new NullReferenceException(nameof(chapter));
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
                            throw new NullReferenceException(nameof(comment));
                        }
                        comments.Add(comment);
                    }
                }

                var demographics = new List<Demographic>();
                if (demographicsIds != null)
                {
                    foreach (var demographicId in demographicsIds)
                    {
                        var demographic = await dbContext.Demographics.FirstOrDefaultAsync(c => c.Id == demographicId);
                        if (demographic == null)
                        {
                            throw new NullReferenceException(nameof(demographic));
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
                    throw new ArgumentException(nameof(id));
                }
                dbContext.Titles.Remove(title);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<TitleRequest> SubmitRequest(string titleId)
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

        public async Task<TitleRequest> ReviewRequest(string requestId, bool isApproved)
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
