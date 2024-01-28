using MangaHomeService.Models;
using MangaHomeService.Models.Entities;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public class BackgroundJobsService(IDbContextFactory<MangaHomeDbContext> contextFactory) : IBackgroundJobsService
    {
        public async Task<bool> FetchPopularTitlesInWeek()
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var currentTime = DateTime.UtcNow;

            var topTitlesWeekly = await dbContext.ViewsCounts.Where(v => currentTime.AddDays(-7).Date <= currentTime.Date)
                .OrderByDescending(v => v.Views).Take(Constants.FeaturedTitlesCount).ToListAsync();
            foreach (var title in topTitlesWeekly)
            {
                var featuredTitle = new FeaturedTitle
                {
                    Title = title.Title,
                    FeaturedCategory = (int)Enums.FeaturedTitlesCategory.PopularInWeek
                };
                await dbContext.FeaturedTitles.AddAsync(featuredTitle);
            }
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> FetchPopularTitlesInMonth()
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var currentTime = DateTime.UtcNow;

            var topTitlesMonthly = await dbContext.ViewsCounts.Where(v => currentTime.AddDays(-30).Date <= currentTime.Date)
                .OrderByDescending(v => v.Views).Take(Constants.FeaturedTitlesCount).ToListAsync();
            foreach (var title in topTitlesMonthly)
            {
                var featuredTitle = new FeaturedTitle
                {
                    Title = title.Title,
                    FeaturedCategory = (int)Enums.FeaturedTitlesCategory.PopularInMonth
                };
                await dbContext.FeaturedTitles.AddAsync(featuredTitle);
            }
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> FetchPopularTitlesInYear()
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var currentTime = DateTime.UtcNow;

            var topTitlesYearly = await dbContext.ViewsCounts.Where(v => currentTime.AddDays(-365).Date <= currentTime.Date)
                .OrderByDescending(v => v.Views).Take(Constants.FeaturedTitlesCount).ToListAsync();
            foreach (var title in topTitlesYearly)
            {
                var featuredTitle = new FeaturedTitle
                {
                    Title = title.Title,
                    FeaturedCategory = (int)Enums.FeaturedTitlesCategory.PopularInMonth
                };
                await dbContext.FeaturedTitles.AddAsync(featuredTitle);
            }
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> FetchPopularTitlesOfAllTime()
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var currentTime = DateTime.UtcNow;

            var topTitlesAllTime = await dbContext.Titles.OrderByDescending(v => v.Views).Take(Constants.FeaturedTitlesCount).ToListAsync();
            foreach (var title in topTitlesAllTime)
            {
                var featuredTitle = new FeaturedTitle
                {
                    Title = title,
                    FeaturedCategory = (int)Enums.FeaturedTitlesCategory.PopularInMonth
                };
                await dbContext.FeaturedTitles.AddAsync(featuredTitle);
            }
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}