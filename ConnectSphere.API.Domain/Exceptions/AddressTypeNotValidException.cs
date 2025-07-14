using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Exceptions
{
    public class AddressTypeNotValidException : DomainModelInvalidException
    {
        internal AddressTypeNotValidException() { }
        internal AddressTypeNotValidException(string message) : base(message) { }
        internal AddressTypeNotValidException(string message, Exception inner) : base(message, inner) { }
    }
}
