using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class Search
    {
        [Required]
        public required string Keyword { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public required int PageNumber { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public required int PageSize { get; set; }
    }
}
