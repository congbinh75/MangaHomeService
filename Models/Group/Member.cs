using static MangaHomeService.Utils.Enums;

namespace MangaHomeService.Models
{
    public class Member : BaseEntity
    {
        public required User User { get; set; }
        public required Group Group { get; set; }
        public required int Role { get; set; } = (int)GroupRole.Member;
    }
}
