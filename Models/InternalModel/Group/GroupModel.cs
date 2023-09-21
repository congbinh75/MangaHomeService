namespace MangaHomeService.Models
{
    public class GroupModel : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProfilePicture { get; set; }
        public List<Member> Members { get; set; }
    }
}
