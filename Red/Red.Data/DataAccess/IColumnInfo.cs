using System.Data;
using System.Linq;

namespace Red.Data.DataAccess
{
    public interface IColumnInfo
    {
        string Name { get; }
        DbType DataType { get; }
        bool IsNullable { get; }
    }

    public static class ExtensionMethodsOnIColumnInfo
    {
        public static bool IsSomeString(this IColumnInfo info)
        {
            var stringTypes = new[]
            {
                DbType.AnsiString,
                DbType.AnsiStringFixedLength,
                DbType.String,
                DbType.StringFixedLength
            };

            return stringTypes.Contains(info.DataType);
        }
    }
}