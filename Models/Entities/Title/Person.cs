using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class Person : BaseEntity
    {
        [Required]
        [MaxLength(64)]
        public required string Name { get; set; }

        [Required]
        public string? Image { get; set; } = string.Empty;

        [MaxLength(512)]
        public string? Description { get; set; } = string.Empty;

        [Required]
        public required ICollection<Title> AuthoredTitles { get; set; } = [];

        [Required]
        public required ICollection<Title> IllustratedTitles { get; set; } = [];

        [Required]
        public bool IsApproved { get; set; }
    }
}
