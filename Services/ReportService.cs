using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public interface IReportService
    {
        public Task<Report> Get(string id);
        public Task<ICollection<Report>> GetAll(int nums = Constants.ReportsPerPage, int page = 0);
        public Task<Report> Add(string id, string reason, string note, Type type);
        public Task<Report> Update(string id, string? reason = null, string? note = null);
        public Task<bool> Delete(string id);
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

        public async Task<ICollection<Report>> GetAll(int nums = Constants.ReportsPerPage, int page = 0)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            return await dbContext.Reports.Skip(page * nums).Take(nums).ToListAsync();
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

        public async Task<bool> Delete(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var report = await dbContext.Reports.FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException(nameof(Report));
            dbContext.Reports.Remove(report);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
