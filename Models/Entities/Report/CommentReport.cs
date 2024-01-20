using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class CommentReport : Report
    {
        [Required]
        public required Comment Comment { get; set; }
    }
}