using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class UploadPage
    {
        [Required]
        public required string ChapterId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Number { get; set; }

        [Required]
        public required IFormFile File { get; set; }
    }
}
