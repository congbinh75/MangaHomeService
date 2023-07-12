namespace MangaHomeService.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string EmailConfirmed { get; set; }
        public string ProfilePicture { get; set; }
        public int Role { get; set; }

        public User() { }

        public User(string id, string name, string email, string password, string emailConfirmed, string profilePicture, int role)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            EmailConfirmed = emailConfirmed;
            ProfilePicture = profilePicture;
            Role = role;
        }
    }
}
