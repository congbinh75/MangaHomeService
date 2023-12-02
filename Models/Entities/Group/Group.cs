using MangaHomeService.Utils;

namespace MangaHomeService.Models.Entities
{
    public class Group : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Member> Members { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];

        public void CheckUploadContions()
        {
            if (!IsApproved)
            {
                throw new NotApprovedException(Name);
            }
            if (!IsActive)
            {
                throw new GroupNotActiveException(Name);
            }
        }
    }
}
