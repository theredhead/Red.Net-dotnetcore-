using System.Collections.Generic;
using System.Data;

namespace Red.Data.DataAccess
{
    public interface IDatabaseInfo
    {
        ISqlDialect Dialect { get; }
        string Name { get; }
        IEnumerable<ITableInfo> Tables { get; }

        void Discover(IDbConnection connection);
    }
}