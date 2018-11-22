using System;
using System.Data;

namespace Red.Data.DataAccess
{
    public abstract class DbDialect : ISqlDialect
    {
        public abstract string QuoteName(string name);

        protected virtual FetchPredicate CreatePredicate(string text, params object[] arguments)
        {
            return new FetchPredicate(text, arguments);
        }

        public FetchPredicate Between<T>(IColumnInfo column, T minimumValue, T maximumValue) 
            => CreatePredicate($"{QuoteName(column.Name)} BETWEEN ? AND ?", minimumValue, maximumValue);

        public FetchPredicate StartsWith(IColumnInfo column, string needle) 
            => CreatePredicate($"LEFT({QuoteName(column.Name)}, {needle.Length}) = ?", needle);

        public FetchPredicate EndsWith(IColumnInfo column, string needle) 
            => CreatePredicate($"RIGHT({QuoteName(column.Name)}, {needle.Length}) = ?", needle);

        public FetchPredicate Contains(IColumnInfo column, string needle) 
            => CreatePredicate($"INSTR({QuoteName(column.Name)}, ?) > 0", needle);

        public FetchPredicate Equals(IColumnInfo column, object needle) 
            => CreatePredicate($"{QuoteName(column.Name)} = ?", needle);
        
        public DbType GetDbTypeFromString(string dataTypeName) 
            => Enum.TryParse(dataTypeName, true, out DbType result)
                ? result
                : DbType.Object;
    }
}
