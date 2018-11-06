using System.Data;

namespace Red.Data.DataAccess
{
    public interface ISqlDialect
    {
        string QuoteName(string name);

        FetchPredicate Between<T>(IColumnInfo column, T minimumValue, T maximumValue);
        FetchPredicate StartsWith(IColumnInfo column, string needle);
        FetchPredicate EndsWith(IColumnInfo column, string needle);
        FetchPredicate Contains(IColumnInfo column, string needle);
        FetchPredicate Equals(IColumnInfo column, object needle);

        DbType GetDbTypeFromString(string dataTypeName);
    }
}