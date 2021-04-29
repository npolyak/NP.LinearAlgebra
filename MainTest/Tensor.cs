using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MainTest
{

    public class Tensor<T>
    {
        private TensorImpl<T> Impl { get; }

        private int FullOffset { get; set; }

        internal Span<T> TheSpan => TheBaseSpan.Slice(FullOffset, ChunkSize);

        private Span<T> TheBaseSpan => Impl._array;

        public int Len { get; init; }

        private int DimensionOffset
        {
            get; set;
        }

        public Span<int> BaseShapeDimensions =>
            Impl.ShapeDimensions;

        public Span<int> ShapeDimensions =>
            BaseShapeDimensions.Slice(DimensionOffset);

        public Span<int> BaseShapeDimensionSizes =>
            Impl.ShapeDimensionSizes;

        public Span<int> ShapeDimensionSizes =>
           BaseShapeDimensionSizes.Slice(DimensionOffset);

        public int NumberDimensions => ShapeDimensions.Length;

        public int TotalNumberDimensions => BaseShapeDimensions.Length;

        public int NumberOfItemsInDimension => ShapeDimensions[0];
        public int ChunkSize => ShapeDimensionSizes[0];

        public Tensor(IEnumerable<T> collection, IEnumerable<int> dimensions = null)
        {
            Impl = new TensorImpl<T>(collection, dimensions);
        }

        public Tensor(T val, IEnumerable<int> dimensions)
        {
            Impl = new TensorImpl<T>(val, dimensions);
        }

        private Tensor(TensorImpl<T> impl)
        {
            Impl = impl;
        }

        public Tensor(Tensor<T> t) : this(t.Impl)
        {

        }

        public Tensor(IEnumerable<int> dimensions) : this(new TensorImpl<T>(dimensions))
        {

        }


        public void CheckLen(int len)
        {
            this.ShapeDimensions.ToArray().CheckShape(len);
        }

        public void SetSpan(Span<T> span)
        {
            this.CheckLen(span.Length);

            span.CopyTo(TheSpan);
        }

        public void CheckEqualDimensions(Span<int> shapeDimensions)
        {
            if (!ShapeDimensions.EqualSpans(shapeDimensions))
            {
                throw new Exception($"Shape dimensions '{this.ShapeDimensions.ToStr()}' are not the same as '{shapeDimensions.ToStr()}'");
            }
        }

        public void CheckEqualDimensions<TIn>(Tensor<TIn> tensor)
        {
            CheckEqualDimensions(tensor.ShapeDimensions);
        }

        public Tensor<T> Reshape(params int[] shapeDimentions)
        {
            return new Tensor<T>(new TensorImpl<T>(this.Impl, shapeDimentions));
        }

        public T this[int i] => TheSpan[i];

        public T Get(params int[] idxes)
        {
            int dimLen = NumberDimensions;

            if (idxes.Length != dimLen)
            {
                throw new Exception($"Programming Error:, length of passed index is {idxes.Length}, but should be {dimLen}");
            }

            int totalIdx = 0;
            for (int i = 0; i < dimLen; i++)
            {
                int currentIdx = idxes[i];

                if (currentIdx >= ShapeDimensions[i])
                {
                    throw new Exception($"Programming Error: index for dimension {i} is {idxes[i]} - greater or equal than the corresponding dimension size - {ShapeDimensions[i]}.");
                }

                int currentDimensionSize = ShapeDimensionSizes[i + 1];

                totalIdx += currentIdx * currentDimensionSize;
            }

            return TheSpan[totalIdx];
        }

        private bool IsInt =>
            typeof(T) == typeof(int);

        private string PrintFormat =>
            IsInt ? "{0:#}" : "{0:#.0000}";

        private bool IsNextToBottom =>
            DimensionOffset == BaseShapeDimensions.Length - 1;

        private bool IsBottom =>
            DimensionOffset == BaseShapeDimensions.Length;

        public IList<Tensor<T>> GetSubTensors()
        {
            List<Tensor<T>> result = new List<Tensor<T>>();

            int nextDimensionOffset = DimensionOffset + 1;

            for (int i = 0; i < NumberOfItemsInDimension; i++)
            {
                Tensor<T> t = new Tensor<T>(this.Impl);
                t.DimensionOffset = nextDimensionOffset;

                t.FullOffset = i * t.ChunkSize + this.FullOffset;

                result.Add(t);
            }

            return result;
        }

        public string ToFullStr(string format = null)
        {
            if (format == null)
            {
                format = PrintFormat;
            }

            if (IsBottom)
            {
                return string.Format(format, this.TheSpan[0]);
            }

            string separator = null;

            if (IsNextToBottom)
            {
                separator = ", ";
            }
            else
            {
                separator = new string('\n', TotalNumberDimensions - DimensionOffset - 1);
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('[');
            bool isFirstIteration = true;
            int shift = DimensionOffset + 1;

            foreach (Tensor<T> subTensor in GetSubTensors())
            {
                if (!isFirstIteration)
                {
                    stringBuilder.Append(separator);

                    if (shift > 0 && (!IsNextToBottom))
                    {
                        stringBuilder.Append(' ', shift);
                    }
                }

                stringBuilder.Append(subTensor.ToFullStr(format));

                isFirstIteration = false;
            }
            stringBuilder.Append(']');

            return stringBuilder.ToString();
        }

        public static Tensor<double> operator +(Tensor<T> t1, Tensor<T> t2)
        {
            return t1.DoDoubleCellByCell(t2, (item1, item2) => item1 + item2);
        }


        public static Tensor<double> operator +(Tensor<T> t1, double scalar)
        {
            return t1.DoDoubleScalarCellByCell(scalar, (item1, item2) => item1 + item2);
        }


        public static Tensor<double> operator *(Tensor<T> t1, Tensor<T> t2)
        {
            return t1.DoDoubleCellByCell(t2, (item1, item2) => item1 * item2);
        }


        public static Tensor<double> operator *(Tensor<T> t1, double scalar)
        {
            return t1.DoDoubleScalarCellByCell(scalar, (item1, item2) => item1 * item2);
        }



        public static Tensor<double> operator -(Tensor<T> t1, Tensor<T> t2)
        {
            return t1.DoDoubleCellByCell(t2, (item1, item2) => item1 - item2);
        }


        public static Tensor<double> operator -(Tensor<T> t1, double scalar)
        {
            return t1.DoDoubleScalarCellByCell(scalar, (item1, item2) => item1 - item2);
        }


        public static Tensor<double> operator /(Tensor<T> t1, Tensor<T> t2)
        {
            return t1.DoDoubleCellByCell(t2, (item1, item2) => item1 / item2);
        }


        public static Tensor<double> operator /(Tensor<T> t1, double scalar)
        {
            return t1.DoDoubleScalarCellByCell(scalar, (item1, item2) => item1 / item2);
        }

        public static Tensor<int> operator %(Tensor<T> t1, Tensor<T> t2)
        {
            return t1.DoIntCellByCell(t2, (item1, item2) => item1 % item2);
        }

        public static Tensor<int> operator %(Tensor<T> t1, int module)
        {
            return t1.DoIntScalarCellByCell(module, (item1, item2) => item1 % item2);
        }
    }
}
