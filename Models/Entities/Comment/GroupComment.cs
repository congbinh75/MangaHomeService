using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class GroupComment : Comment
    {
        [Required]
        public required Group Group { get; set; }
    }
}