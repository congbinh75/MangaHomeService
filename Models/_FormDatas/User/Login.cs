using MangaHomeService.Utils;

namespace MangaHomeService.Models.FormDatas.User
{
    public class Login
    {
        public string? Username { get; set; }
        public string? Password { get; set; }

        public void Validate()
        {
            if (!string.IsNullOrEmpty(Username))
            {
                throw new InvalidInputException(nameof(Username));
            }

            if (!string.IsNullOrEmpty(Password))
            {
                throw new InvalidInputException(nameof(Password));
            }
        }
    }
}
