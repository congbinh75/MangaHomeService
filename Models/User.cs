using System.ComponentModel.DataAnnotations.Schema;

namespace MangaHomeService.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? ProfilePicture { get; set; }
        public int Role { get; set; }
        public byte[]? Salt { get; set; }


        public User() { }
        public User(string name, string email, string password, int role, string? id = null, 
            string? profilePicture = null, bool emailConfirmed = false)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            ProfilePicture = profilePicture;
            EmailConfirmed = emailConfirmed;
            Role = role;
        }
    }
}
