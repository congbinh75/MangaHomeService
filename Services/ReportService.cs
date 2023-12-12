using MangaHomeService.Models;
using MangaHomeService.Models.Entities;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public interface IReportService
    {
        public Task<Report> Get(string id);
        public Task<ICollection<object>> GetAll(string keyword = "", int pageSize = Constants.ReportsPerPage, int pageNumber = 0, int? reportType = null, bool isReviewedIncluded = true);
        public Task<Report> Add(string id, string reason, string note, Type type);
        public Task<Report> Update(string id, string? reason = null, string? note = null);
        public Task<bool> Remove(string id);
    }

    public class ReportService : IReportService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public ReportService(IDbContextFactory<MangaHomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Report> Get(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var report = await dbContext.Reports.FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException(nameof(Report));
            return report;
        }

        public async Task<ICollection<object>> GetAll(string keyword = "", int pageSize = Constants.ReportsPerPage, int pageNumber = 0, int? reportType = null, bool isReviewedIncluded = true)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            if (isReviewedIncluded)
            {
                if (reportType == (int)Enums.RequestType.Group)
                {
                    var results = await dbContext.Requests.OfType<GroupRequest>()
                        .Include(r => r.Group).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                    return (ICollection<object>)results.Where(r => r.Group.Name.Contains(keyword)).ToList();
                }
                else if (reportType == (int)Enums.RequestType.Member)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<MemberRequest>()
                        .Include(r => r.Group).Include(r => r.Member)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (reportType == (int)Enums.RequestType.Title)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<TitleRequest>()
                        .Include(r => r.Group).Include(r => r.Title)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (reportType == (int)Enums.RequestType.Chapter)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<ChapterRequest>()
                        .Include(r => r.Group).Include(r => r.Chapter)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (reportType == (int)Enums.RequestType.Author)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<AuthorRequest>()
                        .Include(r => r.Author).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (reportType == (int)Enums.RequestType.Group)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<ArtistRequest>()
                        .Include(r => r.Artist).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (reportType == null)
                {
                    var requests = (ICollection<object>)await dbContext.Requests.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                    foreach (var request in requests)
                    {
                        if (request is GroupRequest groupRequest)
                        {
                            await dbContext.Entry(groupRequest).Reference(r => r.Group).LoadAsync();
                        }
                        else if (request is MemberRequest memberRequest)
                        {
                            await dbContext.Entry(memberRequest).Reference(r => r.Group).LoadAsync();
                            await dbContext.Entry(memberRequest).Reference(r => r.Member).LoadAsync();
                            await dbContext.Entry(memberRequest.Member).Reference(m => m.User).LoadAsync();
                        }
                        else if (request is TitleRequest titleRequest)
                        {
                            await dbContext.Entry(titleRequest).Reference(r => r.Title).LoadAsync();
                            await dbContext.Entry(titleRequest).Reference(r => r.Group).LoadAsync();
                        }
                        else if (request is ChapterRequest chapterRequest)
                        {
                            await dbContext.Entry(chapterRequest).Reference(r => r.Chapter).LoadAsync();
                            await dbContext.Entry(chapterRequest).Reference(r => r.Group).LoadAsync();
                        }
                        else if (request is AuthorRequest authorRequest)
                        {
                            await dbContext.Entry(authorRequest).Reference(r => r.Author).LoadAsync();
                        }
                        else if (request is ArtistRequest artistRequest)
                        {
                            await dbContext.Entry(artistRequest).Reference(r => r.Artist).LoadAsync();
                        }
                        else
                        {
                            //TO BE FIXED
                            throw new Exception();
                        }
                    }
                    return requests;
                }
                else
                {
                    //TO BE FIXED
                    throw new Exception();
                }
            }
            else
            {
                if (reportType == (int)Enums.RequestType.Group)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<GroupRequest>()
                        .Include(r => r.Group).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (reportType == (int)Enums.RequestType.Member)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<MemberRequest>()
                        .Include(r => r.Group).Include(r => r.Member)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (reportType == (int)Enums.RequestType.Title)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<TitleRequest>()
                        .Include(r => r.Group).Include(r => r.Title)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (reportType == (int)Enums.RequestType.Chapter)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<ChapterRequest>()
                        .Include(r => r.Group).Include(r => r.Chapter)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (reportType == (int)Enums.RequestType.Author)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<AuthorRequest>()
                        .Include(r => r.Author).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (reportType == (int)Enums.RequestType.Group)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<ArtistRequest>()
                        .Include(r => r.Artist).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (reportType == null)
                {
                    var requests = (ICollection<object>)await dbContext.Requests.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                    foreach (var request in requests)
                    {
                        if (request is GroupRequest groupRequest)
                        {
                            await dbContext.Entry(groupRequest).Reference(r => r.Group).LoadAsync();
                        }
                        else if (request is MemberRequest memberRequest)
                        {
                            await dbContext.Entry(memberRequest).Reference(r => r.Group).LoadAsync();
                            await dbContext.Entry(memberRequest).Reference(r => r.Member).LoadAsync();
                            await dbContext.Entry(memberRequest.Member).Reference(m => m.User).LoadAsync();
                        }
                        else if (request is TitleRequest titleRequest)
                        {
                            await dbContext.Entry(titleRequest).Reference(r => r.Title).LoadAsync();
                            await dbContext.Entry(titleRequest).Reference(r => r.Group).LoadAsync();
                        }
                        else if (request is ChapterRequest chapterRequest)
                        {
                            await dbContext.Entry(chapterRequest).Reference(r => r.Chapter).LoadAsync();
                            await dbContext.Entry(chapterRequest).Reference(r => r.Group).LoadAsync();
                        }
                        else if (request is AuthorRequest authorRequest)
                        {
                            await dbContext.Entry(authorRequest).Reference(r => r.Author).LoadAsync();
                        }
                        else if (request is ArtistRequest artistRequest)
                        {
                            await dbContext.Entry(artistRequest).Reference(r => r.Artist).LoadAsync();
                        }
                        else
                        {
                            //TO BE FIXED
                            throw new Exception();
                        }
                    }
                    return requests;
                }
                else
                {
                    //TO BE FIXED
                    throw new Exception();
                }
            }
        }

        public async Task<Report> Add(string id, string reason, string note, Type type)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            if (type == typeof(UserReport))
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new NotFoundException(nameof(User));
                var report = new UserReport
                {
                    Reason = reason,
                    Note = note,
                    User = user,
                    IsReviewed = false
                };
                await dbContext.Reports.AddAsync(report);
                await dbContext.SaveChangesAsync();
                return report;
            }
            else if (type == typeof(TitleReport))
            {
                var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == id) ?? throw new NotFoundException(nameof(User));
                var report = new TitleReport
                {
                    Reason = reason,
                    Note = note,
                    Title = title,
                    IsReviewed = false
                };
                await dbContext.Reports.AddAsync(report);
                await dbContext.SaveChangesAsync();
                return report;
            }
            else if (type == typeof(GroupReport))
            {
                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == id) ?? throw new NotFoundException(nameof(Group));
                var report = new GroupReport
                {
                    Reason = reason,
                    Note = note,
                    Group = group,
                    IsReviewed = false
                };
                await dbContext.Reports.AddAsync(report);
                await dbContext.SaveChangesAsync();
                return report;
            }
            else if (type == typeof(ChapterReport))
            {
                var chapter = await dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == id) ?? throw new NotFoundException(nameof(Chapter));
                var report = new ChapterReport
                {
                    Reason = reason,
                    Note = note,
                    Chapter = chapter,
                    IsReviewed = false
                };
                await dbContext.Reports.AddAsync(report);
                await dbContext.SaveChangesAsync();
                return report;
            }
            else
            {
                throw new ArgumentException(type.ToString());
            }
        }

        public async Task<Report> Update(string id, string? reason = null, string? note = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var report = await dbContext.Reports.FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException(nameof(Report));
            report.Reason = reason ?? report.Reason;
            report.Note = note ?? report.Note;
            await dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<bool> Remove(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var report = await dbContext.Reports.FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException(nameof(Report));
            dbContext.Reports.Remove(report);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
