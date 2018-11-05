using System;
using System.Reflection;

namespace Red.Core
{
    /// <summary>
    /// Provides sanity checking utility
    /// </summary>
    public static class Sanity
    {
        /// <summary>
        /// Enforce that the given predicate is true or throw an exception of the given type
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="messageFormat"></param>
        /// <param name="replacements"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="???"></exception>
        public static void Enforce<T>(bool predicate, string messageFormat = null, Replacements replacements=null) where T : Exception
        {
            if (predicate) return;

            if (messageFormat == null)
                throw GetException<T>();

            if (replacements == null)
                throw GetException<T>(messageFormat);

            throw GetException<T>(replacements.ApplyTo(messageFormat));
        }
        
        /// <summary>
        /// Get an Exception subtype instance with either the given or the default message  
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="MissingMethodException"></exception>
        private static Exception GetException<T>(string message = null) where T : Exception
        {
            var types = string.IsNullOrEmpty(message)
                ? new Type[] { }
                : new Type[] { typeof(string) }; 

            var args= string.IsNullOrEmpty(message)
                ? new object[] { }
                : new object[] { message }; 

            var ctor = typeof(T).GetConstructor(
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                callConvention: CallingConventions.HasThis,
                types: types,
                modifiers: new ParameterModifier[0]);
    
            if (ctor == null)
                throw new MissingMethodException($"Constructor not found trying to create {typeof(T).Name}.");

            return message == null
                ? ctor.Invoke(new object[] { }) as Exception
                : ctor.Invoke(new object[] {message}) as Exception;
        }
    }
}