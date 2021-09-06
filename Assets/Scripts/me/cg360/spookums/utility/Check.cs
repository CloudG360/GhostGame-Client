using System;

namespace me.cg360.spookums.utility
{
    public class Check
    {
        /**
         * For use in json, config, and Data API
         * situations where a specific property that is required
         * is missing.
         *
         * @param obj - The object which represents the property that is missing.
         * @param loc - A string descriptor of the holding object.
         * @param name - The name of the property.
         *
         * @return the parameter "obj"
         */
        public static T MissingProperty<T>(T obj, String loc, String name) {
            if(IsNull(obj)) throw new InvalidOperationException($"'{loc}' is missing a valid '{name}' property.");
            return obj;
        }

        /**
         * Checks if a parameter is null in a cleaner way, throwing an IllegalArgumentException if true.
         * @param obj - the parameter that is potentially null.
         * @param name - the name of the parameter.
         *
         * @return the parameter "obj"
         */
        public static T NullParam<T>(T obj, String name) {
            if(IsNull(obj)) throw new ArgumentNullException($"'{name}' cannot be null.");
            return obj;
        }

        /**
         * Assets that a value is between two numbers.
         * @param val the value being checked.
         * @param lowerBound the lower bound checked (inclusive)
         * @param upperBound the upper bound checked (inclusive)
         * @param name the name of the variable/property.
         *
         * @return the parameter "val"
         */
        public static int InclusiveBounds(int val, int lowerBound, int upperBound, String name) {
            InclusiveLowerBound(val, lowerBound, name);
            InclusiveUpperBound(val, upperBound, name);
            return val;
        }

        public static int InclusiveLowerBound(int val, int bound, String name) {
            if(val < bound) throw new InvalidOperationException($"{name} is out of bounds (val = {val} | Lower = {bound})");
            return val;
        }

        public static int InclusiveUpperBound(int val, int bound, String name) {
            if(val > bound) throw new InvalidOperationException($"{name} is out of bounds (val = {val} | Upper = %{bound})");
            return val;
        }

        public static bool IsNull(Object obj) {
            return obj == null;
        }
    }
}