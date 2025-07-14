using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Exceptions
{
    public class PersonTypeNotValidException : DomainModelInvalidException
    {
        internal PersonTypeNotValidException() { }
        internal PersonTypeNotValidException(string message) : base(message) { }
        internal PersonTypeNotValidException(string message, Exception inner) : base(message, inner) { }
    }
}
