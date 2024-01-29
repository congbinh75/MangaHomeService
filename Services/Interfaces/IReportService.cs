using MangaHomeService.Models.Entities;
using MangaHomeService.Models.InputModels;
using MangaHomeService.Utils;

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
}