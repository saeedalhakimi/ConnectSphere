namespace ConnectSphere.API.Domain.Exceptions
{
    public class CountryNotValidException : DomainModelInvalidException
    {
        internal CountryNotValidException() { }
        internal CountryNotValidException(string message) : base(message) { }
        internal CountryNotValidException(string message, Exception inner) : base(message, inner)
        {
        }
    }

}
