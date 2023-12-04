using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class UpdateChapter
    {
        [Required]
        public required string Id { get; set; }

        [Range(0, int.MaxValue)]
        public double? Number { get; set; }

        public string? TitleId { get; set; }

        public string? GroupId { get; set; }

        public string? VolumeId { get; set; }

        public string? LanguageId { get; set; }
    }
}
