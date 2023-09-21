namespace MangaHomeService.Models
{
    public class PermissionModel : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Role> Roles { get; set; }
    }
}
