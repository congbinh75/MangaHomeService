namespace MangaHomeService.Services
{
    public interface IBackgroundJobsService
    {
        public Task<bool> FetchPopularTitlesInWeek();
        public Task<bool> FetchPopularTitlesInMonth();
        public Task<bool> FetchPopularTitlesInYear();
        public Task<bool> FetchPopularTitlesOfAllTime();
    }
}