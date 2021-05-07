using NP.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MainTest
{
    public class SubTensorIter<T> : IEnumerator<Tensor<T>>
    {
        private Tensor<T> ParentTensor { get; }

        public int Level => ParentTensor.Level + 1;

        int _currentIdx;

        public int CurrentIdx => _currentIdx;

        public int ChunkSize => ParentTensor.SubTensorChunkSize;

        public Tensor<T> Current { get; private set; }

        object IEnumerator.Current => Current;

        public SubTensorIter(Tensor<T> parentTensor)
        {
            ParentTensor = parentTensor;

            Reset();
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            int newIdx;
            if (_currentIdx < 0)
            {
                newIdx = 0;
            }
            else
            {
                newIdx = _currentIdx + 1;
            }

            if (newIdx >= ParentTensor.NumberSubTensors)
            {
                return false;
            }

            _currentIdx = newIdx;

            Current = new Tensor<T>(ParentTensor, Level, ParentTensor.StartOffset + _currentIdx * ChunkSize);

            return true;
        }

        public void Reset()
        {
            Current = null;
            _currentIdx = -1;
        }
    }

    public class Tensor<T> : IEnumerable<Tensor<T>>
    {
        private TensorData<T> TheTensorData { get; }

        public Permutation TheDimensionPermutation
        {
            get => TheTensorData.TheDimensionsPermutations;

            set => TheTensorData.TheDimensionsPermutations = value;
        }

        private T[] TheArray => TheTensorData.TheArray;

        int _level = 0;
        public int Level 
        { 
            get => _level;
            private set
            {
                if (_level == value)
                    return;

                if (value >= TheTensorData.TotalSpaceDimensions.Length)
                {
                    throw new ProgrammingError($"Iteration Level cannot be equal or exceed the number of tensor dimensions");
                }

                _level = value;
            }
        }

        public int RemainingDepth => TheTensorData.TotalSpaceDimensions.Length - Level;

        public T TensorStartValue => TheArray[StartOffset];

        public int StartOffset { get; private set; }

        public Span<int> SpaceDimensions => TheTensorData.TotalSpaceDimensions.AsSpan().Slice(Level);

        public Span<int> SpaceDimensionSizes => TheTensorData.TotalShapeDimensionChunkSizes.AsSpan().Slice(Level);

        public int NumberSubTensors => SpaceDimensions[0];

        public int SubTensorChunkSize => SpaceDimensionSizes[0];

        // total length of the sub-collection in array cells
        public int CurrentTensorCellLength => NumberSubTensors * SubTensorChunkSize;
        
        /// <summary>
        /// offset within the array that is a non-including end boundary within the sub-collection
        /// </summary>
        public int EndOffset => StartOffset + CurrentTensorCellLength;

        public Tensor(IEnumerable<T> collection, IEnumerable<int> dimensions = null, int level = 0, int start = 0)
        {
            TheTensorData = new TensorData<T>(collection, dimensions);

            Level = level;
            StartOffset = start;
        }

        public Tensor(T val, IEnumerable<int> dimensions, int level = 0, int start = 0)
        {
            TheTensorData = new TensorData<T>(val, dimensions);

            Level = level;
            StartOffset = start;
        }

        internal Tensor(TensorData<T> tensorData, int level = 0, int start = 0)
        {
            TheTensorData = tensorData;

            Level = level;
            StartOffset = start;
        }

        public Tensor(Tensor<T> t, int level = 0, int start = 0) : this(t.TheTensorData, level, start)
        {

        }

        public Tensor(IEnumerable<int> dimensions, int level = 0, int start = 0) : this(new TensorData<T>(dimensions), level, start)
        {

        }

        public Tensor<T> Reshape(params int[] shapeDimensions)
        {
            TensorData<T> tensorData = new TensorData<T>(this.TheTensorData, shapeDimensions);

            return new Tensor<T>(tensorData);
        }

        private SubTensorIter<T>  GetSubTensorIter()
        {
            return new SubTensorIter<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetSubTensorIter();
        }

        IEnumerator<Tensor<T>> IEnumerable<Tensor<T>>.GetEnumerator()
        {
            return GetSubTensorIter();
        }
    }
}
