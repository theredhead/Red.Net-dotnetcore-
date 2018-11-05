using System;
using System.Collections.Generic;
using System.Data;

namespace Red.Data.DataAccess
{
    public static class ExtensionMethodsOnIDbCommand
    {
        /// <summary>
        /// Execute a command, returning a lazy enumerable using the given factory function for each element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExecuteEnumerable<T>(this IDbCommand command, Func<IDataRecord, T> factory)
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                    yield return factory(reader);
            }
        }
        /// <summary>
        /// Execute a command, returning a lazy enumerable of some ILoadable subclass
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExecuteEnumerable<T>(this IDbCommand command) where T : ILoadable<IDataRecord>, new()
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var record = new T();
                    record.Load(reader);
                    yield return record;
                }
            }
        }

        /// <summary>
        /// Generic ExecuteScalar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public static T ExecuteScalar<T>(this IDbCommand command) where T : struct
        {
            return (T)Convert.ChangeType(command.ExecuteScalar(), typeof(T));
        }
    }
}