using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainTest
{
    public static class ArrayUtils
    {
        public static IEnumerable<T> ToEnumerable<T>(this Span<T> span)
        {
            return span.ToArray();
        }    

        public static string ToStr<T>(this IEnumerable<T> coll, Func<T, bool> specialValCondition = null)
        {
            if (coll == null)
                return "null";

            if (coll.Count() == 0)
            {
                return "[]";
            }    

            string ToStrBasedOnCondition(T val)
            {
                if (specialValCondition == null)
                    return val.ToString();

                if (specialValCondition.Invoke(val))
                {
                    return ": ";
                }

                return val.ToString();
            }

            return $"[{coll.Skip(1).Aggregate(ToStrBasedOnCondition(coll.FirstOrDefault()), (currentStr, val) => currentStr + ", " + ToStrBasedOnCondition(val))}]";
        }

        public static string ToStr<T>(this Span<T> coll, Func<T, bool> specialValCondition = null)
        {
            return coll.ToEnumerable().ToStr(specialValCondition);
        }
    }
}
