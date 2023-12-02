using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class UpdateUser
    {
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        public IFormFile? ProfilePicture { get; set; }
    }
}
