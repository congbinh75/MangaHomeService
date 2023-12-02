using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class AddChapterLanguage
    {
        [Required]
        [MaxLength(32)]
        public required string Name { get; set; }

        [Required]
        public required IFormFile Logo { get; set; }
    }
}
