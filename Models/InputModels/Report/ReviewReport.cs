using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class ReviewReport
    {
        [Required]
        public required string Id { get; set; }
    }
}
