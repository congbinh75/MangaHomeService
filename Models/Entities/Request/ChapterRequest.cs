using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{ 
    public class ChapterRequest : Request
    {
        [Required]
        public required Chapter Chapter { get; set; }

        [Required]
        public required Group Group { get; set; }
    }
}