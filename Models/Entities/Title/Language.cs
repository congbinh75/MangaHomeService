using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class Language : BaseEntity
    {
        [Required]
        [MaxLength(32)]
        public required string Name { get; set; }

        [Required]
        public required string Logo { get; set; }
    }
}
