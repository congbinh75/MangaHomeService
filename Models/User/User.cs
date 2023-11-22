using MangaHomeService.Utils;

namespace MangaHomeService.Models
{
    public class User : BaseEntity
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required bool IsEmailConfirmed { get; set; } = false;
        public string? ProfilePicture { get; set; } = string.Empty;
        public required int Role { get; set; } = (int)Enums.Role.User;
        public required byte[] Salt { get; set; } = [];
        public List<Group> Groups { get; set; } = [];
        public List<Title> UpdateFeed { get; set; } = [];
        public required bool IsBanned { get; set; } = false;
    }
}
