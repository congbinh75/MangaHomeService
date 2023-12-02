using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class ChapterTracking
    {
        [Required]
        public required User User { get; set; }

        [Required]
        public required Chapter Chapter { get; set; }
    }
}
