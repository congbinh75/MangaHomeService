using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class GetFeaturedTitle
    {
        [Required]
        [Range(0, 3)]
        public required int Category { get; set; }
    }
}
