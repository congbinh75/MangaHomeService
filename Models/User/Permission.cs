namespace MangaHomeService.Models
{
    public class Permission : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public List<Role> Roles { get; set; }

        public Permission() { }
        public Permission(string name, string description, bool isActive, List<Role> roles)
        {
            Name = name;
            Description = description;
            IsActive = isActive;
            Roles = roles;
        }
    }
}
