using System.Collections.Generic;
using System.Data;

namespace Red.Data.DataAccess
{
    public interface ITableInfo
    {
        FetchRequest CreateFetchRequest();
        string Name { get; }
        IEnumerable<IColumnInfo> PrimaryKey { get; }
        IEnumerable<IColumnInfo> Columns { get; }
        void Discover(IDbConnection connection);
        PredicateBuilder Where();
    }
}