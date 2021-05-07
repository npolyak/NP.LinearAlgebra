using NP.Utilities;
using NP.Utilities.BasicInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MainTest
{
    public class ArraySubIter<T> : ArraySubCollection<T>, IEnumerator<T>
    {
        int _currentIdx;

        public int CurrentIdx => _currentIdx;

        public T Current => TheArray[_currentIdx];

        object IEnumerator.Current => Current;

        public ArraySubIter(ArraySubCollection<T> collection) : base(collection)
        {
            Reset();
        }

        public void Dispose()
        {
            // remove the reference to array
            TheArray = null;
        }

        public bool MoveNext()
        {

            int newIdx;
            if (_currentIdx < 0)
            {
                newIdx = StartOffset;
            }
            else
            {
                newIdx = _currentIdx + SubArrayChunkSize;
            }

            if (newIdx >= EndOffset)
            {
                return false;
            }

            _currentIdx = newIdx;

            return true;
        }

        public void Reset()
        {
            _currentIdx = -1;
        }


        public ArraySubCollection<T> GetSubArray(int len, int chunkSize)
        {
            return new ArraySubCollection<T>(TheArray, chunkSize, _currentIdx, len);
        }
    }

    /// <summary>
    /// assumes that the array represents and multi-dimensional matrix 
    /// so that numbers in each row/column... are spaced evenly.
    /// </summary>
    public class ArraySubCollection<T> : IEnumerable<T>, ICopyable<ArraySubCollection<T>>
    {
        public T[] TheArray{ get; protected set; }

        /// <summary>
        /// start offset of the sub-collection within the array
        /// </summary>
        public int StartOffset { get; private set; }

        // total number of sub-collection cells
        public int NumberSubArrays { get; private set; }

        // total length of the sub-collection in array cells
        public int NumberOfCellsInCurrentArray => NumberSubArrays * SubArrayChunkSize;

        /// <summary>
        /// offset within the array that is a non-including end boundary within the sub-collection
        /// </summary>
        public int EndOffset => StartOffset + NumberOfCellsInCurrentArray;

        // size of the chunk to skip within the array between sub-collection cells
        public int SubArrayChunkSize { get; private set; }


        public ArraySubCollection(T[] array, int chunkSize = 1, int start = 0, int length = -1)
        {
            this.TheArray = array;
            this.SubArrayChunkSize = chunkSize;
            this.StartOffset = start;
            this.NumberSubArrays = length == -1 ? (TheArray.Length - start)/SubArrayChunkSize : length;
        }

        public ArraySubCollection(ArraySubCollection<T> tensorSpan)
        {
            this.CopyFrom(tensorSpan);
        }

        public void CopyFrom(ArraySubCollection<T> source)
        {
            this.TheArray = source.TheArray;
            this.StartOffset = source.StartOffset;
            this.NumberSubArrays = source.NumberSubArrays;
            this.SubArrayChunkSize = source.SubArrayChunkSize;
        }

        public void DeepCopyTo(ArraySubCollection<T> target)
        {
            target.CopyFrom(this);

            target.TheArray = new T[this.TheArray.Length];

            Array.Copy(TheArray, target.TheArray, TheArray.Length);
        }

        public void DeepCopyContentFrom(IEnumerable<T> collection)
        {
            int collLen = collection?.Count() ?? 0;
            if (collLen != NumberSubArrays)
            {
                throw new Exception($"Collection length '{collLen}' does not match the length of the TensorSpan: '{NumberSubArrays}'.");
            }

            IEnumerator<T> enumerator = collection.GetEnumerator();
            for (int i = 0; i < NumberSubArrays; i++)
            {
                enumerator.MoveNext();

                this[i] = enumerator.Current;
            }
        }

        public ArraySubIter<T> GetIter()
        {
            return new ArraySubIter<T>(this);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetIter();
        }

        private int CalculateIdx(int i)
        {
            if (i >= NumberSubArrays || i < 0)
            {
                throw new ProgrammingError($"Idx '{i}' is outside of the boundaries [0; {NumberSubArrays})");
            }

            return StartOffset + i * SubArrayChunkSize;
        }

        public T this[int i]
        {
            get => TheArray[CalculateIdx(i)];
            set => TheArray[CalculateIdx(i)] = value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetIter();

        public override string ToString()
        {
            return "Start: " + StartOffset +
                    "\nEnd: " + EndOffset +
                    "\nChunk Size: " + SubArrayChunkSize +
                    "\nLength: " + NumberSubArrays;
        }

        public string GetResultStr()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("{");

            for(int i = 0; i < NumberSubArrays; i++)
            {
                if (i > 0)
                {
                    stringBuilder.Append(", ");
                }

                stringBuilder.Append(this[i]);
            }

            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }

    }
}
