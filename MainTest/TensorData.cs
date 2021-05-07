using NP.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace MainTest
{
    /// <summary>
    /// TensorData = TensorImpl + Permutation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TensorData<T>
    {
        TensorImpl<T> _impl;
        internal TensorImpl<T> Impl
        {
            get => _impl;
            set
            {
                if (_impl == value)
                    return;

                _impl = value;

                SetDefaultDimensionsPermutations();
            }
        }

        internal T[] TheArray => Impl.TheArray;

        Permutation _dimensionPermutations;
        public Permutation TheDimensionsPermutations
        {
            get => _dimensionPermutations;
            set
            {
                if (_dimensionPermutations == value)
                    return;

                _dimensionPermutations = value;

                if (NumberDimensions > NumberDimensionsImpl)
                {
                    throw new ProgrammingError($"Number dimensions '{NumberDimensions}' cannot be larger than the number of implemented dimensions '{NumberDimensionsImpl}'.");
                }

                TotalSpaceDimensions = Impl.ShapeDimensions.Permutate(TheDimensionsPermutations).ToArray();
                TotalShapeDimensionChunkSizes = Impl.ShapeDimensionSizes.Permutate(TheDimensionsPermutations).ToArray();

                TotalTensorSize = TotalSpaceDimensions.Aggregate(1, (i1, i2) => i1 * i2);
            }
        }

        public int[] TotalSpaceDimensions { get; private set; }

        public int[] TotalShapeDimensionChunkSizes { get; private set; }

        public int TotalTensorSize { get; private set; }

        // read only copy
        public int[] SpaceDimensionsImpl => Impl.ShapeDimensions.ToArray();

        public int NumberDimensionsImpl => SpaceDimensionsImpl.Length;

        public int NumberDimensions => _dimensionPermutations.Length;

        private void SetDefaultDimensionsPermutations()
        {
            TheDimensionsPermutations = new Permutation(NumberDimensionsImpl);
        }

        public TensorData(IEnumerable<T> collection, IEnumerable<int> dimensions = null)
        {
            Impl = new TensorImpl<T>(collection, dimensions);
        }

        public TensorData(T val, IEnumerable<int> dimensions)
        {
            Impl = new TensorImpl<T>(val, dimensions);
        }

        private TensorData(TensorImpl<T> impl, IEnumerable<int> dimensions = null)
        {
            Impl = new TensorImpl<T>(impl, dimensions);
        }

        public TensorData(TensorData<T> t, IEnumerable<int> dimensions = null) : this(t.Impl, dimensions)
        {

        }

        public TensorData(IEnumerable<int> dimensions) : this(new TensorImpl<T>(dimensions))
        {

        }

        public TensorData<T> Reshape(params int[] shapeDimensions)
        {
            var result = new TensorData<T>(new TensorImpl<T>(this.Impl, shapeDimensions));

            return result;
        }
    }
}
