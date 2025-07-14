using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Exceptions
{
    public class EmailAddressNotValidException : DomainModelInvalidException
    {
        internal EmailAddressNotValidException() { }
        internal EmailAddressNotValidException(string message) : base(message) { }
        internal EmailAddressNotValidException(string message, Exception inner) : base(message, inner) { }
    }
}
