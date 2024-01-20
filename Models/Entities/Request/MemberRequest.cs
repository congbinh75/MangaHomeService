using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class MemberRequest : Request
    {
        [Required]
        public required Group Group { get; set; }

        [Required]
        public required Member Member { get; set; }
    }
}