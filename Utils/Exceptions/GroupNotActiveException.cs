namespace MangaHomeService.Utils
{ 
    public class GroupNotActiveException : Exception
    {
        public GroupNotActiveException() { }
        public GroupNotActiveException(string message) : base(message) { }
    }
}