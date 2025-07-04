using ConnectSphere.API.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.Services
{
    public interface IErrorHandlingService
    {
        OperationResult<T> ResourceAlreadyExists<T>(string key);
        OperationResult<T> ResourceCreationFailed<T>();
        OperationResult<T> HandleException<T>(Exception ex, string correlationId);
        OperationResult<T> HandleCancelationToken<T>(OperationCanceledException ex, string correlationId);
    }
}
