using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class UserReport : Report
    {
        [Required]
        public required User User { get; set; }
    }
}