namespace ConnectSphere.API.Domain.Exceptions
{
    public class PhoneNumberTypeNotValideException : DomainModelInvalidException
    {
        internal PhoneNumberTypeNotValideException() { }
        internal PhoneNumberTypeNotValideException(string message) : base(message) { }
        internal PhoneNumberTypeNotValideException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
