using MangaHomeService.Utils;
using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class GetAllRequest
    {
        [Required]
        public required string Keyword { get; set; }

        [Required]
        public required bool IsReviewedIncluded { get; set; }

        public required int? RequestType { get; set; }

        [Range(0, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(0, int.MaxValue)]
        public int PageSize { get; set; } = Constants.TitlesPerPage;
    }
}