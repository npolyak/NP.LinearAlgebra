using NP.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MainTest
{
    public class Permutation : IEnumerable<int>
    {
        private int[] _array;

        public int Length => _array.Length;

        /// TODO
        //public bool IsEven =>;

        private void CheckPermutation()
        {
            if (Length == 0)
            {
                throw new ProgrammingError("Length of a permutation cannot be zero.");
            }

            for (int i = 0; i < Length; i++)
            {
                if (!_array.Contains(i))
                {
                    throw new ProgrammingError($"Invalid Permutation: it does not contain '{i}'");
                }
            }
        }

        public int this[int i] => _array[i];

        public IEnumerator<int> GetEnumerator()
        {
            return (_array as IEnumerable<int>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _array.GetEnumerator();
        }

        public Permutation(int len)
        {
            _array = new int[len];

            for(int i = 0; i < len; i++)
            {
                _array[i] = i;
            }
        }

        private void CheckVal(int val)
        {
            if (val >= Length)
            {
                throw new ProgrammingError($"Permutation value {val} cannot be greater or equal to permutation length {Length}");
            }
        }


        public Permutation(params int[] vals)
        {
            _array = new int[vals.Length];

            for(int i = 0; i < Length; i++)
            {
                int val = vals[i];

                CheckVal(val);

                _array[i] = val;
            }

            CheckPermutation();
        }

        public Permutation SwapIdxes(int idx1, int idx2)
        {
            if (idx1 == idx2)
            {
                return this;
            }

            CheckVal(idx1);
            CheckVal(idx2);

            int[] array = new int[Length];

            Array.Copy(_array, array, Length);

            array[idx1] = _array[idx2];
            array[idx2] = _array[idx1];

            return new Permutation(array);
        }

        public Permutation CircularShift(int shift)
        {
            shift = shift % Length;

            if (shift == 0)
            {
                return this;
            }

            if (shift < 0)
            {
                shift = Length + shift;
            }

            int[] array = new int[Length];

            for (int i = 0; i < shift; i++)
            {
                array[i] = _array[Length - shift + i];
            }

            for (int i = shift; i < Length; i++)
            {
                array[i] = _array[i - shift];
            }

            return new Permutation(array);
        }

        public override string ToString()
        {
            return _array.ToStr(null, "{", "}");
        }

        public override bool Equals(object obj)
        {
            if (obj is Permutation p)
            {
                if (this.Length != p.Length)
                    return false;

                return _array.SequenceEqual(p._array);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int result = 0;

            int logBase = 1;
            for(int i = 0; i < Length; i++)
            {
                result += i * logBase;

                logBase *= 10;
            }

            return result;
        }
    }

    public static class PermutationHelper
    {
        public static IEnumerable<T> Permutate<T>(this T[] array, Permutation p)
        {
            if (p.Length > array.Length)
            {
                throw new ProgrammingError($"Permutation lenght '{p.Length}' cannot be larger than the array length '{array.Length}'");
            }

            foreach (int idx in p)
            {
                yield return array[idx];
            }
        }
    }
}
