using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Exceptions
{
    public class GovernmentalInfoNotValidException : DomainModelInvalidException
    {
        internal GovernmentalInfoNotValidException() { }
        internal GovernmentalInfoNotValidException(string message) : base(message) { }
        internal GovernmentalInfoNotValidException(string message, Exception inner) : base(message, inner) { }
    }
}
