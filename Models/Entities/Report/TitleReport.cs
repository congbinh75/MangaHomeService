using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{

    public class TitleReport : Report
    {
        [Required]
        public required Title Title { get; set; }
    }
}