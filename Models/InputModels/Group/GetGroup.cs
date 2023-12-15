using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class GetGroup
    {
        [Required]
        public required string Id { get; set; }
    }
}
