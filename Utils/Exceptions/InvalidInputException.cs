namespace MangaHomeService.Utils
{ 
        public class InvalidInputException : Exception
    {
        public InvalidInputException() { }
        public InvalidInputException(string message) : base(message) { }
    }
}