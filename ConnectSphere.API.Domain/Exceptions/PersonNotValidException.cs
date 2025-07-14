namespace ConnectSphere.API.Domain.Exceptions
{
    public class PersonNotValidException : DomainModelInvalidException
    {
        internal PersonNotValidException(){}
        internal PersonNotValidException(string message) : base(message) {}
        internal PersonNotValidException(string message, Exception inner) : base(message, inner){}
    }
}
