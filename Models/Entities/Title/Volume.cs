using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class Volume : BaseEntity
    {
        [Required]
        [Range(0, int.MaxValue)]
        public required int Number { get; set; }

        [MaxLength(128)]
        public string? Name { get; set; } = string.Empty;

        [Required]
        public required Title Title { get; set; }
    }
}
