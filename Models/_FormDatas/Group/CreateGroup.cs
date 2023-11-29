namespace MangaHomeService.Models.FormDatas
{
    public class CreateGroup
    {
        public required string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public ICollection<string>? MembersIds { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Name)) 
            { 
                throw new ArgumentNullException(nameof(Name));
            }
        }
    }
}
