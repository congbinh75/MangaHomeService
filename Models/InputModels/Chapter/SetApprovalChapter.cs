using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class SetApprovalChapter
    {
        [Required]
        public required string ChapterId { get; set; }

        [Required]
        public required bool IsApproved { get; set; }
    }
}
