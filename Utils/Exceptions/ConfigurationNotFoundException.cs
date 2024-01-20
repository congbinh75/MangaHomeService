namespace MangaHomeService.Utils
{ 
    public class ConfigurationNotFoundException : Exception
    {
        public ConfigurationNotFoundException() { }
        public ConfigurationNotFoundException(string message) : base(message) { }
    }
}