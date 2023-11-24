namespace MangaHomeService.Utils
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() { }
        public InvalidCredentialsException(string message) : base(message) { }
    }
    public class EmailNotConfirmedException : Exception
    {
        public EmailNotConfirmedException() { }
        public EmailNotConfirmedException(string message) : base(message) { }
    }
    public class EmailAlreadyRegisteredException : Exception
    {
        public EmailAlreadyRegisteredException() { }
        public EmailAlreadyRegisteredException(string message) : base(message) { }
    }
    public class AlreadyReviewedException : Exception
    {
        public AlreadyReviewedException() { }
        public AlreadyReviewedException(string message) : base(message) { }
    }
    public class AlreadyApprovedException : Exception
    {
        public AlreadyApprovedException() { }
        public AlreadyApprovedException(string message) : base(message) { }
    }
    public class NotFoundException : Exception
    {
        public NotFoundException() { }
        public NotFoundException(string message) : base(message) { }
    }
    public class NotApprovedException : Exception
    {
        public NotApprovedException() { }
        public NotApprovedException(string message) : base(message) { }
    }
    public class GroupNotActiveException : Exception
    {
        public GroupNotActiveException() { }
        public GroupNotActiveException(string message) : base(message) { }
    }
    public class ConfigurationNotFoundException : Exception
    {
        public ConfigurationNotFoundException() { }
        public ConfigurationNotFoundException(string message) : base(message) { }
    }
    public class InvalidInputException : Exception
    {
        public InvalidInputException() { }
        public InvalidInputException(string message) : base(message) { }
    }
}
