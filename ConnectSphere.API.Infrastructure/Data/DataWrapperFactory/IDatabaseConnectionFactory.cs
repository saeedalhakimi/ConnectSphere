using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Infrastructure.Data.DataWrapperFactory
{
    public interface IDatabaseConnectionFactory
    {
        Task<IDatabaseConnection> CreateConnectionAsync(string connectionString, CancellationToken cancellationToken);
    }
}
