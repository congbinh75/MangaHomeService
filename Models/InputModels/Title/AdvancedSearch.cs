using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class AdvancedSearch
    {
        public string? Name { get; set; }

        public string? Author { get; set; }

        public string? Artist { get; set; }

        public List<string>? GenreIds { get; set; }

        public List<string>? ThemeIds { get; set; }

        public string? OriginalLanguageId { get; set; }

        public List<string>? LanguageIds { get; set; }

        public List<string>? Statuses { get; set; }

        [Required]
        public bool SortByLastest { get; set; }

        [Required]
        public bool SortByHottest { get; set; }

        [Required]
        public int PageNumber { get; set; }

        [Required]
        public int PageSize { get; set; }
    }
}
