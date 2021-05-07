using System;
using System.Collections.Generic;

namespace MainTest
{
    public static class MultiDimensionalArrayIteratorTest
    {
        public static void Test()
        {
            int len = 30;
            int[] array = new int[len];

            for(int i = 0; i < len; i++)
            {
                array[i] = i;
            }

            TensorData<int> tensorData = new TensorData<int>(array, new[] { 2, 3, 5 });
            tensorData.TheDimensionsPermutations = new Permutation(0,1,2);

            ArraySubCollection<int> coll1 = new ArraySubCollection<int>(array, tensorData.TotalShapeDimensionChunkSizes[0], 0, tensorData.TotalSpaceDimensions[0]);

            ArraySubIter<int> iter1 = coll1.GetIter();
            List<int> arrayOfNumbers = new List<int>();
            while (iter1.MoveNext())
            {
                Console.Write("[");
                var iter2 = iter1.GetSubArray(tensorData.TotalSpaceDimensions[1], tensorData.TotalShapeDimensionChunkSizes[1]).GetIter();
                while (iter2.MoveNext())
                {
                    var iter3 = iter2.GetSubArray(tensorData.TotalSpaceDimensions[2], tensorData.TotalShapeDimensionChunkSizes[2]).GetIter();
                    bool isFirst = true;
                    Console.Write("[");
                    while (iter3.MoveNext())
                    {
                        arrayOfNumbers.Add(iter3.Current);
                        
                        if (!isFirst)
                        {
                            Console.Write(", ");
                        }

                        Console.Write(iter3.Current);

                        isFirst = false;
                    }
                    Console.Write("]\n");
                }
                Console.Write("]\n");
            }
        }
    }
}
