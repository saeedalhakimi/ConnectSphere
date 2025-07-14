using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Exceptions
{
    public class BirthDetailsNotValidException : DomainModelInvalidException
    {
        internal BirthDetailsNotValidException() : base("Birth details are not valid.")
        {
        }
        internal BirthDetailsNotValidException(string message) : base(message)
        {
        }
        internal BirthDetailsNotValidException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
