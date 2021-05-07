using System;
using System.Collections.Generic;
using System.Linq;

namespace MainTest
{
    internal class TensorImpl<T>
    {
        internal T[] _array;

        public T[] TheArray => _array;

        public int Len => _array?.Length ?? 0;

        public int NumberDimensions => ShapeDimensions.Length;

        private int[] _shapeDimensions;
        public int[] ShapeDimensions
        {
            get => _shapeDimensions;
            init
            {
                if (_shapeDimensions == value)
                    return;

                _shapeDimensions = value;

                if (_shapeDimensions == null)
                {
                    ShapeDimensionSizes = null;
                }
                else
                {
                    ShapeDimensionSizes = new int[NumberDimensions];

                    int currentDimSize = 1;
                    for (int i = NumberDimensions - 1; i >= 0; i--)
                    {
                        ShapeDimensionSizes[i] = currentDimSize;

                        currentDimSize *= ShapeDimensions[i];
                    }
                }
            }
        }

        public int[] ShapeDimensionSizes { get; init; }

        public TensorImpl(IEnumerable<T> collection, IEnumerable<int> dimensions = null)
        {
            _array = collection.ToArray();

            if (dimensions == null)
            {
                ShapeDimensions = new[] { collection.Count() };
            }
            else 
            {
                ShapeDimensions = dimensions.ToArray();

                CheckShapes();
            }
        }


        public TensorImpl(TensorImpl<T> tensorImpl, IEnumerable<int> dimensions = null) :
            this(tensorImpl._array, dimensions ?? tensorImpl.ShapeDimensions)
        {

        }

        public TensorImpl(IEnumerable<int> dimensions)
        {
            this.ShapeDimensions = dimensions.ToArray();

            int len = ShapeDimensions.TotalSize();

            _array = new T[len];
        }

        public TensorImpl(T val, IEnumerable<int> dimensions) : this(dimensions)
        {
            Array.Fill(_array, val);
        }

        public void CheckLen(int len)
        {
            this.ShapeDimensions.CheckShape(len);
        }

        private void CheckShapes()
        {
            this.CheckLen(Len);
        }

        public TensorImpl<TOut> DeepCopy<TOut>(Func<T, TOut> converter)
        {
            TOut[] array = new TOut[this._array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = converter(_array[i]);
            }

            TensorImpl<TOut> result = new TensorImpl<TOut>(array, this.ShapeDimensions);

            return result;
        }
    }

}
