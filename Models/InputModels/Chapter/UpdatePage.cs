using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class UpdatePage
    {
        [Required]
        public required string Id { get; set; }

        [Required]
        public required string Number { get; set; }
    }
}
