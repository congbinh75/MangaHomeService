using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class RemoveChapter
    {
        [Required]
        public required string Id { get; set; }
    }
}
