using System.Security.Claims;

namespace MangaHomeService.Models
{
    public class User : BaseModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string ProfilePicture { get; set; }
        public int Role { get; set; }
        public byte[]? Salt { get; set; }
        public List<Group> Groups { get; set; }
        public List<Title> UpdateFeed { get; set; }
        public static ClaimsIdentity Identity { get; internal set; }
        public bool IsBanned { get; set; }
    }
}
