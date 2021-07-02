using System;

namespace me.cg360.spookums.utility
{
    public class Check
    {
        public static void MissingProperty(Object obj, String loc, String name) {
            if(IsNull(obj)) throw new InvalidOperationException($"'{loc}' is missing a valid '{name}' property.");
        }

        public static void NullParam(Object obj, String name) {
            if(IsNull(obj)) throw new ArgumentNullException($"'{name}' cannot be null.");
        }

        /**
     * Assets that a value is between two numbers.
     * @param val the value being checked.
     * @param lowerBound the lower bound checked (inclusive)
     * @param upperBound the upper bound checked (inclusive)
     * @param name the name of the variable/property.
     */
        public static void InclusiveBounds(int val, int lowerBound, int upperBound, String name) {
            if((val < lowerBound)) throw new InvalidOperationException($"{name} is out of bounds (val = {val} | Lower = {lowerBound})");
            if((val > upperBound)) throw new InvalidOperationException($"{name} is out of bounds (val = {val} | Upper = %{upperBound})");
        }

        public static bool IsNull(object obj) {
            return obj == null;
        }
    }
}