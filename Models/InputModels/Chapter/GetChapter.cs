using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class GetChapter
    {
        [Required]
        public required string Id { get; set; }
    }
}
