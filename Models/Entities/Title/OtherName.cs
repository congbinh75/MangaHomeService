using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class OtherName : BaseEntity
    {
        [Required]
        [MaxLength(128)]
        public required string Name { get; set; }

        [Required]
        public required Language Language { get; set; }
    }
}
