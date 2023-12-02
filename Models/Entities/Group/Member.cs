using System.ComponentModel.DataAnnotations;
using static MangaHomeService.Utils.Enums;

namespace MangaHomeService.Models.Entities
{
    public class Member : BaseEntity
    {
        [Required]
        public required User User { get; set; }

        [Required]
        public required Group Group { get; set; }

        [Required]
        [Range(0, 2)]
        public required int Role { get; set; } = (int)GroupRole.Member;
    }
}
