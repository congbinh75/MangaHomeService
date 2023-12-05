using MangaHomeService.Utils;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace MangaHomeService.Models.InputModels
{
    public class AdvancedTitleSearch
    {
        public string? Name { get; set; }

        public string? Author { get; set; }

        public string? Artist { get; set; }

        public ICollection<string>? GenreIds { get; set; }

        public ICollection<string>? ThemeIds { get; set; }

        public string? OriginalLanguageId { get; set; }

        public ICollection<string>? LanguageIds { get; set; }

        public ICollection<string>? Statuses { get; set; }

        public bool SortByLastest { get; set; } = false;

        public bool SortByHottest { get; set; } = false;

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = Constants.TitlesPerPage;
    }
}
