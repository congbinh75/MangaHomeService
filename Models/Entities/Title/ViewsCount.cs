using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class ViewsCount
    {
        [Required]
        public required Title Title { get; set; }

        [Required]
        public required int Views { get; set; }

        [Required]
        public required DateTime Date { get; set; }
    }
}
