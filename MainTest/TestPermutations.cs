using System;

namespace MainTest
{
    public static class TestPermutations
    {
        public static void Test()
        {
            Permutation p = new Permutation(5);
            Console.WriteLine(p);

            Permutation p1 = p.SwapIdxes(2, 3);
            Console.WriteLine(p1);

            Permutation p2 = p.CircularShift(2);
            Console.WriteLine(p2);

            Permutation p3 = p.CircularShift(-2);
            Console.WriteLine(p3);

            Permutation pCopy = p3.CircularShift(2);

            if (pCopy.Equals(p))
            {
                Console.WriteLine("CORRECT: Permutations equal");
            }
            else
            {
                Console.WriteLine("INCORRECT: Permutations are different.");
            }
        }
    }
}
