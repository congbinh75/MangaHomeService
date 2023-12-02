using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class Chapter : BaseEntity
    {
        [Required]
        [Range(0, double.MaxValue)]
        public required double Number { get; set; }

        [Required]
        public required Title Title { get; set; }

        [Required]
        public Volume? Volume { get; set; }

        [Required]
        public Language? Language { get; set; }

        [Required]
        public required ICollection<Page> Pages { get; set; } = [];

        [Required]
        public required Group Group { get; set; }

        [Required]
        public required ICollection<Comment> Comments { get; set; } = [];

        [Required]
        public required bool IsApproved { get; set; }
    }
}
