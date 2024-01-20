using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class GroupReport : Report
    {
        [Required]
        public required Group Group { get; set; }
    }
}