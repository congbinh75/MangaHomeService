using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class FeaturedTitle : BaseEntity
    {
        [Required]
        public required Title Title { get; set; }

        [Required]
        public required int FeaturedCategory { get; set; }
    }
}
