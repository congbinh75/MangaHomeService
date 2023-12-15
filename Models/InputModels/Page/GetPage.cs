using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class GetPage
    {
        [Required]
        public required string Id { get; set; }
    }
}
