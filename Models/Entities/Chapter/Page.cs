using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class Page : BaseEntity
    {
        [Required]
        public required Chapter Chapter { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public required int Number { get; set; }

        [Required]
        public required string File { get; set; }
    }
}
