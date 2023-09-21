using System.Security.Claims;

namespace MangaHomeService.Models
{
    public class UserModel : BaseModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? ProfilePicture { get; set; }
        public Role? Role { get; set; }
        public byte[]? Salt { get; set; }
        public List<Group> Groups { get; set; }
        public static ClaimsIdentity? Identity { get; internal set; }
    }
}
