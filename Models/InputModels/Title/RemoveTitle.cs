using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class RemoveTitle
    {
        [Required]
        public required string Id { get; set; }
    }
}
