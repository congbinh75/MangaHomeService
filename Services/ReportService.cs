using MangaHomeService.Models;
using MangaHomeService.Models.Entities;
using MangaHomeService.Models.InputModels;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public interface IReportService
    {
        public Task<Report> Get(string id);
        public Task<ICollection<object>> GetAll(string keyword = "", int pageSize = Constants.ReportsPerPage, int pageNumber = 0, int? reportType = null, bool isReviewedIncluded = true);
        public Task<Report> Add(SubmitReport data);
        public Task<Report> Update(string id, string? reason = null, string? note = null);
        public Task<Report> Review(string id);
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
                if (reportType == (int)Enums.ReportType.Group)
                {
                    var results = await dbContext.Reports.OfType<GroupReport>()
                        .Include(r => r.Group)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                    return (ICollection<object>)results.Where(r => r.Group.Name.Contains(keyword)).ToList();
                }
                else if (reportType == (int)Enums.ReportType.Title)
                {
                    return (ICollection<object>)await dbContext.Reports.OfType<TitleReport>()
                        .Include(r => r.Title)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (reportType == (int)Enums.ReportType.Chapter)
                {
                    return (ICollection<object>)await dbContext.Reports.OfType<ChapterReport>()
                        .Include(r => r.Chapter)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (reportType == (int)Enums.ReportType.User)
                {
                    return (ICollection<object>)await dbContext.Reports.OfType<UserReport>()
                        .Include(r => r.User)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (reportType == null)
                {
                    var requests = (ICollection<object>)await dbContext.Reports.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                    foreach (var request in requests)
                    {
                        if (request is GroupReport groupReport)
                        {
                            await dbContext.Entry(groupReport).Reference(r => r.Group).LoadAsync();
                        }
                        else if (request is TitleReport titleReport)
                        {
                            await dbContext.Entry(titleReport).Reference(r => r.Title).LoadAsync();
                        }
                        else if (request is ChapterReport chapterReport)
                        {
                            await dbContext.Entry(chapterReport).Reference(r => r.Chapter).LoadAsync();
                            await dbContext.Entry(chapterReport.Chapter).Reference(r => r.Group).LoadAsync();
                        }
                        else if (request is UserReport userReport)
                        {
                            await dbContext.Entry(userReport).Reference(r => r.User).LoadAsync();
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

        public async Task<Report> Add(SubmitReport data)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            if (data is UserReportData userReportData)
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userReportData.UserId) ?? throw new NotFoundException(nameof(User));
                var report = new UserReport
                {
                    Reason = userReportData.Reason,
                    Note = userReportData.Note,
                    User = user,
                    IsReviewed = false
                };
                await dbContext.Reports.AddAsync(report);
                await dbContext.SaveChangesAsync();
                return report;
            }
            else if (data is TitleReportData titleReportData)
            {
                var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == titleReportData.TitleId) ?? throw new NotFoundException(nameof(User));
                var report = new TitleReport
                {
                    Reason = titleReportData.Reason,
                    Note = titleReportData.Note,
                    Title = title,
                    IsReviewed = false
                };
                await dbContext.Reports.AddAsync(report);
                await dbContext.SaveChangesAsync();
                return report;
            }
            else if (data is GroupReportData groupReportData)
            {
                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupReportData.GroupId) ?? throw new NotFoundException(nameof(Group));
                var report = new GroupReport
                {
                    Reason = groupReportData.Reason,
                    Note = groupReportData.Note,
                    Group = group,
                    IsReviewed = false
                };
                await dbContext.Reports.AddAsync(report);
                await dbContext.SaveChangesAsync();
                return report;
            }
            else if (data is ChapterReportData chapterReportData)
            {
                var chapter = await dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == chapterReportData.ChapterId) ?? throw new NotFoundException(nameof(Chapter));
                var report = new ChapterReport
                {
                    Reason = chapterReportData.Reason,
                    Note = chapterReportData.Note,
                    Chapter = chapter,
                    IsReviewed = false
                };
                await dbContext.Reports.AddAsync(report);
                await dbContext.SaveChangesAsync();
                return report;
            }
            else
            {
                throw new ArgumentException(data.GetType().Name);
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

        public async Task<Report> Review(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var report = await dbContext.Reports.FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException(nameof(Report));
            
            if (report is GroupReport groupReport)
            {
                await dbContext.Entry(groupReport).Reference(r => r.Group).LoadAsync();
            }
            else if (report is TitleReport titleReport)
            {
                await dbContext.Entry(titleReport).Reference(r => r.Title).LoadAsync();
            }
            else if (report is ChapterReport chapterReport)
            {
                await dbContext.Entry(chapterReport).Reference(r => r.Chapter).LoadAsync();
            }
            else if (report is UserReport userReport)
            {
                await dbContext.Entry(userReport).Reference(r => r.User).LoadAsync();
            }
            else
            {
                //TO BE FIXED
                throw new Exception();
            }

            if (report.IsReviewed)
            {
                throw new AlreadyReviewedException();
            }

            report.IsReviewed = true;

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
