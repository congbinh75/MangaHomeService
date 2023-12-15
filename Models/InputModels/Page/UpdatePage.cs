using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class UpdatePage
    {
        [Required]
        public required string Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public required int Number { get; set; }
    }
}
