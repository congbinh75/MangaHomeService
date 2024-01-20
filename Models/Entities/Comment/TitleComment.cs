using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class TitleComment : Comment
    {
        [Required]
        public required Title Title { get; set; }
    }
}