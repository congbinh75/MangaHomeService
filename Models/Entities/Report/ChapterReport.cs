using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class ChapterReport : Report
    {
        [Required]
        public required Chapter Chapter { get; set; }
    }
}