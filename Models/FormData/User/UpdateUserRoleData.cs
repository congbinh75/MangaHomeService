namespace MangaHomeService.Models.FormData
{
    public class UpdateUserRoleData
    {
        public string UserId { get; set; }
        public List<string> RoleIds { get; set; }
    }
}
