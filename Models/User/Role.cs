namespace MangaHomeService.Models
{
    public class Role : BaseModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<Permission> Permissions { get; set; }

        public Role() { }
        public Role(string? name, string? description, List<Permission> permissions)
        {
            Name = name;
            Description = description;
            Permissions = permissions;
        }
    }
}
