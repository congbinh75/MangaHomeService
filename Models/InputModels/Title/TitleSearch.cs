using MangaHomeService.Utils;
using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class TitleSearch
    {
        [Required]
        public required string Keyword { get; set; }

        [Range(0, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(0, int.MaxValue)]
        public int PageSize { get; set; } = Constants.TitlesPerPage;
    }
}
