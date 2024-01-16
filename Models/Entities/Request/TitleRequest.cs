using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{ 
    public class TitleRequest : Request
    {
        [Required]
        public required Title Title { get; set; }

        [Required]
        public required Group Group { get; set; }
    }
}