namespace MangaHomeService.Models
{
    public class GroupRole : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<GroupPermission> Permissions { get; set; }
    }
}
