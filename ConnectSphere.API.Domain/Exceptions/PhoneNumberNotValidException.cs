namespace ConnectSphere.API.Domain.Exceptions
{
    public class PhoneNumberNotValidException : DomainModelInvalidException
    {
        internal PhoneNumberNotValidException() { }
        internal PhoneNumberNotValidException(string message) : base(message) { }
        internal PhoneNumberNotValidException(string message, Exception inner) : base(message, inner) { }
    }
}
