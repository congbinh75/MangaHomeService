using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class GetChaptersByTitle
    {
        [Required]
        public required string TitleId { get; set; }

        [Required]
        public required int PageSize { get; set; }

        [Required]
        public required int PageNumber { get; set; }
    }
}
