using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class RemoveGroup
    {
        [Required]
        public required string GroupId { get; set; }
    }
}
