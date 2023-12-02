using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class CreateTitle
    {
        [Required]
        [MaxLength(128)]
        public required string Name { get; set; }

        [MaxLength(512)]
        public string? Description { get; set; }
    }
}
