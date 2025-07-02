using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Common.Enums
{
    public enum ErrorCode
    {
        UnknownError = 999,
        InvalidInput = 1000,
        ConflictError = 1001,
        InternalServerError = 1002,
        OperationCancelled = 1003,
        NotFound = 1004,
        Unauthorized = 1005,
        BadRequest = 1006,
        Locked = 1007,
        NoResult = 1008,
        InvalidData = 1009,
        OperationCanceled = 1010,
        InvalidOperation = 1011,
        Database = 1012,
        AuthorizationError = 1013,
    }
}

