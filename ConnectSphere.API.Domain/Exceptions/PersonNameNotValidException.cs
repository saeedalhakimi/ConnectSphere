using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Exceptions
{
    public class PersonNameNotValidException : DomainModelInvalidException
    {
        internal PersonNameNotValidException() { }
        internal PersonNameNotValidException(string message) : base(message) { }
        internal PersonNameNotValidException(string message, Exception inner) : base(message, inner) { }
    }
}
