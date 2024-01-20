using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class RequestChapter
    {
        [Required]
        public required string ChapterId { get; set; }

        [Required]
        public required string GroupId { get; set; }

        [MaxLength(256)]
        public string? SubmitNote { get; set; }
    }
}
