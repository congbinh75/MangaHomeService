using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace MangaHomeService.Models
{
    public class User : BaseModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? ProfilePicture { get; set; }
        public Role Role { get; set; }
        public byte[]? Salt { get; set; }
        public static ClaimsIdentity? Identity { get; internal set; }

        public User() { }
        public User(string name, string email, string password, bool emailConfirmed, string profilePicture, byte[] salt, Role role)
        {
            Name = name;
            Email = email;
            Password = password;
            EmailConfirmed = emailConfirmed;
            ProfilePicture = profilePicture;
            Salt = salt;
            Role = role;
        }
    }
}
