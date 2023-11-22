using MangaHomeService.Utils;

namespace MangaHomeService.Models
{
    public class Group : BaseModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ProfilePicture { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        public List<Member>? Members { get; set; }
        public List<Comment>? Comments { get; set; }

        public void CheckUploadContions()
        {
            if (!IsApproved)
            {
                throw new NotApprovedException(Name ?? "");
            }
            if (!IsActive)
            {
                throw new GroupNotActiveException(Name ?? "");
            }
        }
    }
}
