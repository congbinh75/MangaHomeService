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
        public int Role { get; set; }
        public byte[]? Salt { get; set; }
        public static ClaimsIdentity Identity { get; internal set; }

        public User() { }
        public User(string name, string email, string password, int role)
        {
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }
    }
}
