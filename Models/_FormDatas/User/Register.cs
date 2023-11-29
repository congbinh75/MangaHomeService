using MangaHomeService.Utils;

namespace MangaHomeService.Models.FormDatas
{
    public class Register
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new InvalidInputException(nameof(Name));
            }

            if (string.IsNullOrEmpty(Email))
            {
                throw new InvalidInputException(nameof(Email));
            }

            if (string.IsNullOrEmpty(Password))
            {
                throw new InvalidInputException(nameof(Password));
            }
        }
    }
}
