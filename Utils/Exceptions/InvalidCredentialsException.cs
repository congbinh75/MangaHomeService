namespace MangaHomeService.Utils
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() { }
        public InvalidCredentialsException(string message) : base(message) { }
    }
}
