using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class ChapterComment : Comment
    {
        [Required]
        public required Chapter Chapter { get; set; }
    }
}