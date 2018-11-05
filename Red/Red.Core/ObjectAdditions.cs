namespace Red.Core
{
    /// <summary>
    /// Provides useful utility extension methods
    /// </summary>
    public static class ObjectAdditions
    {
        /// <summary>
        /// Determines if the instance is assignable to the generic type T.
        /// </summary>
        /// <param name="anObject"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsAssignableTo<T>(this object anObject)
        {
            return !(anObject is null) && typeof(T).IsAssignableFrom(anObject.GetType());
        }
    }
}