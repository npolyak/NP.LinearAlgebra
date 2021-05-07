using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainTest
{
    public static class DimensionsUtils
    {
        public static Func<int, bool> DimensionCondition = (i) => i > 0;

        public static string DimensionsToStr(this IEnumerable<int> dimensions)
        {
            return dimensions.ToStr(i => !DimensionCondition(i));
        }

        public static int TotalSize(this IEnumerable<int> dimensions) =>
            dimensions.Where(DimensionCondition).Prod();

        public static void CheckShape(this IEnumerable<int> dimensions, int arrayLenth)
        {
            int minDimensionLen = dimensions.TotalSize();

            if (arrayLenth % minDimensionLen != 0)
            {
                throw new Exception($"ERROR: dimensions {dimensions.DimensionsToStr()} do not match the length {arrayLenth} of the array");
            }
        }
    }
}
