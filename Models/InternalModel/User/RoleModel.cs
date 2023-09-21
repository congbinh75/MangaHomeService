namespace MangaHomeService.Models
{
    public class RoleModel : BaseModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
