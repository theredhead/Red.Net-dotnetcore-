using System.Collections.Generic;
using System.Data;
using System.Linq;
using Red.Data.DataAccess.MySql;

namespace Red.Data.DataAccess.Base
{
    public abstract class Database : IDatabaseInfo
    {
        public ITableInfo this[string name] => _Tables[name];

        protected string ConnectionString { get; private  set; }

        public ISqlDialect Dialect { get; }
        public string Name { get; protected set; }
        
        protected Dictionary<string,ITableInfo> _Tables { get; } = new Dictionary<string,ITableInfo>{};
        public IEnumerable<ITableInfo> Tables => _Tables.Values;
        
        public abstract IDbConnection CreateConnection();

        public abstract void Discover();

        protected Database(ISqlDialect dialect)
        {
            Dialect = dialect;
        }

        protected Database(string connectionString, ISqlDialect dialect) : this(dialect)
        {
            ConnectionString = connectionString;
        }

        public virtual FetchRequest CreateFetchRequest()
        {
            return new FetchRequest(this);
        }
    }

    public class TableInfo : ITableInfo
    {
        public string Name { get; internal set;}
        public IDatabaseInfo Database { get; internal set; }
        public IEnumerable<IColumnInfo> PrimaryKey { get; } = new IColumnInfo[] {};
        public IEnumerable<IColumnInfo> Columns { get; } = new IColumnInfo[] {};
        
        public FetchRequest CreateFetchRequest()
        {
            return new FetchRequest(Database)
            {
                Table = this
            };
        }

        public virtual void Discover(IDbConnection connection)
        {
            throw new System.NotImplementedException();
        }

        public FetchRequest Search(string[] searchTexts, IEnumerable<IColumnInfo> columnsToSearch)
        {
            var request = CreateFetchRequest();
            request.Table = this;

            foreach (var text in searchTexts)
            {
                foreach (var column in columnsToSearch)
                {
                    request.AddPredicate(Database.Dialect.Contains(column, text));
                }
            }

            return request;
        }
        public FetchRequest Search(string[] searchTexts)
        {
            var searchableColumns = Columns.Where((c) => c.IsSomeString()).ToArray();
            return Search(searchTexts, searchableColumns);
        }
        public PredicateBuilder Where()
        {
            var request = new FetchRequest(this.Database)
            {
                Table = this
            };
            return request.Where();
        }
    }

    public class ColumnInfo : IColumnInfo
    {
        public ITableInfo Table { get; internal set; }
        public string Name { get; internal set;  }
        public DbType DataType { get; internal set; }
        public bool IsNullable { get; internal set; }
    }
}