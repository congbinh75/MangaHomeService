namespace MangaHomeService.Utils
{ 
    public class EmailAlreadyRegisteredException : Exception
    {
        public EmailAlreadyRegisteredException() { }
        public EmailAlreadyRegisteredException(string message) : base(message) { }
    }
}