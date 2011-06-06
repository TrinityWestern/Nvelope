namespace Nvelope
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Tests whether 2 objects are equal, but handles nulls gracefully
        /// </summary>
        /// <remarks>Stolen from Clojure</remarks>
        public static bool Eq(this object obj, object other)
        {
            // Handle nulls
            if (obj == null)
                return (other == null);     
            
            if (other == null)
                return false;

            return obj.Equals(other);
        }

        /// <summary>
        /// Like ToString, but it handles nulls and gives nicer results for
        /// some objects.
        /// </summary>
        public static string Print(this object o) 
        {
            // This function should also work polymorphically
            // Sometimes, we've got variables of type object, but we want them to print 
            // nicer than ToString() for their type (ie, for decimals, ToString() works stupidly)
            // So we do shotgun polymorphism here to take care of that, since we can't 
            // hack into the original types to override their ToString methods

            // Decimals don't do ToString in a reasonable way
            // It's really irritating
            if (o is decimal)
                return ((decimal)o).Print();
            else
                return o.ToStringN();
        }
    }
}