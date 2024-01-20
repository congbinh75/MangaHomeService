using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{ 
    public class GroupRequest : Request
    {
        [Required]
        public required Group Group { get; set; }
    }
}