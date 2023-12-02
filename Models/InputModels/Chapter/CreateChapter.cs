using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class CreateChapter
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int Number { get; set; }

        [Required]
        public required string TitleId { get; set; }

        [Required]
        public required string GroupId { get; set; }

        public string? VolumeId { get; set; }

        public string? LanguageId { get; set; }
    }
}
