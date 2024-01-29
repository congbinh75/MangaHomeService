using MangaHomeService.Models.Entities;
using MangaHomeService.Models.InputModels;
using MangaHomeService.Utils;

namespace MangaHomeService.Services
{
    public interface IRequestService
    {
        public Task<Request> Get(string id);
        public Task<ICollection<object>> GetAll(string keyword = "", int pageNumber = 1, int pageSize = Constants.RequestsPerPage, int? requestType = null, bool isReviewedIncluded = true);
        public Task<Request> Add(SubmitRequest submitRequest);
        public Task<Request> Update(string id, string note);
        public Task<Request> Review(string id, string note, bool isApproved);
        public Task<bool> Remove(string id);
    }
}