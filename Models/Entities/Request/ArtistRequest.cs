using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{ 
    public class ArtistRequest : Request
    {
        [Required]
        public required Person Artist { get; set; }
    }
}