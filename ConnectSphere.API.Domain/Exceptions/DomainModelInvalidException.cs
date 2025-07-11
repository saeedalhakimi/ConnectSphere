using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Exceptions
{
    public class DomainModelInvalidException : Exception
    {
        internal DomainModelInvalidException()
        {
            ValidationErrors = new List<string>();
        }

        internal DomainModelInvalidException(string message) : base(message)
        {
            ValidationErrors = new List<string>();
        }

        internal DomainModelInvalidException(string message, Exception inner) : base(message, inner)
        {
            ValidationErrors = new List<string>();
        }
        public List<string> ValidationErrors { get; }
    }
}
