using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class UpdateUser
    {
        [MinLength(6)]
        [MaxLength(16)]
        public string? Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        public IFormFile? ProfilePicture { get; set; }
    }
}
