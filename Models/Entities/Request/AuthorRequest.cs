using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{ 
    public class AuthorRequest : Request
    {
        [Required]
        public required Person Author { get; set; }
    }
}