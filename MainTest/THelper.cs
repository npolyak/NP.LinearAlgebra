using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainTest
{
    public delegate void RefFunc<TTarget, TSource>(ref TTarget target, TSource source);

    public static class THelper
    {
        public static Tensor<T> Create<T>(this T val, IEnumerable<int> dimensions) =>
            new Tensor<T>(val, dimensions);

        public static Tensor<T> Create<T>
        (
            this IEnumerable<T> collection, 
            IEnumerable<int> dimensions = null) => new Tensor<T>(collection);


        private static Tensor<T> ArangeImpl<T>(double begin, double end, double step)
        {
            if (end < begin)
            {
                step = -Math.Abs(step);
            }

            Type t = typeof(T);

            Converter<double, T> converter = NumericUtils.GetConverterFromDouble<T>();

            int numberSteps = (int) ((end - begin) / step);

            T[] array = new T[numberSteps];
            double currentValue = begin;
            for (int i = 0; i < numberSteps; i++, currentValue += step)
            {
                array[i] = converter.Invoke(currentValue);
            }

            return new Tensor<T>(array);
        }

        public static Tensor<int> Arange(int begin, int end, int step = 1) => 
            ArangeImpl<int>(begin, end, step);

        public static Tensor<float> Arange(float begin, float end, float step = 1) => 
            ArangeImpl<float>(begin, end, step);

        public static Tensor<double> Arange(double begin, double end, double step = 1) => 
            ArangeImpl<double>(begin, end, step);

        public static Tensor<decimal> Arange(decimal begin, decimal end, decimal step = 1) =>
            ArangeImpl<decimal>((double) begin, (double) end, (double) step);

        public static TensorOld<TOut> Cast<TIn, TOut>(this TensorOld<TIn> t, Func<TIn, TOut> converter = null)
        {
            if (converter == null)
            {
                converter = NumericUtils.GetNumericConverter<TIn, TOut>();
            }

            TensorOld<TOut> result = new TensorOld<TOut>(t.ShapeDimensions.ToArray());

            result.SetSpan(t.TheSpan.Select(item => converter(item)));

            return result;
        }


        public static void DoCellByCell<TTarget, TSource>
        (
            this TensorOld<TTarget> target,
            TSource source,
            RefFunc<TTarget, TSource> operation)
        {
            TTarget t = default(TTarget);
            for (int i = 0; i < target.TheSpan.NumberSubArrays; i++)
            {
                operation(ref t, source);

                target.TheSpan[i] = t;
            }
        }


        public static void DoCellByCell<TTarget, TSource>
        (
            this TensorOld<TTarget> target, 
            TensorOld<TSource> source, 
            RefFunc<TTarget, TSource> operation)
        {
            target.CheckEqualDimensions(source);
            TTarget t = default(TTarget);
            for (int i = 0; i < target.TheSpan.NumberSubArrays; i++)
            {
                operation(ref t, source.TheSpan[i]);
                target.TheSpan[i] = t;
            }
        }

        public static TensorOld<TOut> DoCellByCell<TOut, TIn1, TIn2>(this TensorOld<TIn1> t1, TensorOld<TIn2> t2, Func<TIn1, TIn2, TOut> operation)
        {
            t1.CheckEqualDimensions(t2);

            TensorOld<TOut> result = new TensorOld<TOut>(t1.ShapeDimensions.ToArray());

            for(int i = 0; i < t1.TheSpan.NumberSubArrays; i++)
            {
                result.TheSpan[i] = operation(t1.TheSpan[i], t2.TheSpan[i]);
            }

            return result;
        }

        public static TensorOld<TOut> DoScalarCellByCell<TOut, TIn1, TIn2>(this TensorOld<TIn1> t1, TIn2 val2, Func<TIn1, TIn2, TOut> operation)
        {
            TensorOld<TOut> result = new TensorOld<TOut>(t1.ShapeDimensions.ToArray());

            for (int i = 0; i < t1.TheSpan.NumberSubArrays; i++)
            {
                result.TheSpan[i] = operation(t1.TheSpan[i], val2);
            }

            return result;
        }

        public static TensorOld<double> DoDoubleScalarCellByCell<TIn1, TIn2>(this TensorOld<TIn1> t1, TIn2 val2, Func<double, double, double> operation)
        {
            var converter1 = NumericUtils.GetConverterToDouble<TIn1>();
            var converter2 = NumericUtils.GetConverterToDouble<TIn2>();

            double dVal2 = converter2(val2);

            return t1.DoScalarCellByCell(val2, (TIn1 item1, TIn2 item2) => operation(converter1(item1), dVal2));
        }

        public static TensorOld<int> DoIntScalarCellByCell<TIn1, TIn2>(this TensorOld<TIn1> t1, TIn2 val2, Func<int, int, int> operation)
        {
            var converter1 = NumericUtils.GetNumericConverter<TIn1, int>();
            var converter2 = NumericUtils.GetNumericConverter<TIn2, int>();

            int dVal2 = converter2(val2);

            return t1.DoScalarCellByCell(val2, (TIn1 item1, TIn2 item2) => operation(converter1(item1), dVal2));
        }

        public static TensorOld<int> DoIntCellByCell<TIn1, TIn2>(this TensorOld<TIn1> t1, TensorOld<TIn2> t2, Func<int, int, int> operation)
        {
            var converter1 = NumericUtils.GetNumericConverter<TIn1, int>();
            var converter2 = NumericUtils.GetNumericConverter<TIn2, int>();

            return t1.DoCellByCell(t2, (TIn1 item1, TIn2 item2) => operation(converter1(item1), converter2(item2)));
        }

        public static TensorOld<double> DoDoubleCellByCell<TIn1, TIn2>(this TensorOld<TIn1> t1, TensorOld<TIn2> t2, Func<double, double, double> operation)
        {
            var converter1 = NumericUtils.GetConverterToDouble<TIn1>();
            var converter2 = NumericUtils.GetConverterToDouble<TIn2>();

            return t1.DoCellByCell(t2, (TIn1 item1, TIn2 item2) => operation(converter1(item1), converter2(item2)));
        }

        public static TensorOld<double> Dot(this TensorOld<double> t1, TensorOld<double> t2)
        {
            if (t1.NumberDimensions > 2 || t2.NumberDimensions > 2)
            {
                throw new ProgrammingError("Dot product is not defined for tensors more that dimension 2");
            }

            if (t1.NumberDimensions == 1)
            {
                t1 = t1.Reshape(t1.Len, 1);
            }

            if (t2.NumberDimensions == 1)
            {
                t2.Reshape(1, t2.Len);
            }

            if (t1.ShapeDimensions[1] != t2.ShapeDimensions[0])
            {
                throw new ProgrammingError($"number of colums of the first matrix '{t1.ShapeDimensions[1]}' is not equal to the number of rows of the second: '{t2.ShapeDimensions[0]}'");
            }

            int convolutionSize = t1.ShapeDimensions[1];

            TensorOld<double> result = new TensorOld<double>(new[] { t1.ShapeDimensions[0], t2.ShapeDimensions[1] });

            int numRows = t1.ShapeDimensions[0];
            int numCols = t2.ShapeDimensions[1];

            for (int col = 0; col < numCols; col++)
            {
                result.GetTensorSpan(1, col);
                for (int row = 0; row < numRows; row++)
                {
                    ArraySubCollection<double> iter1 = t1.GetTensorSpan(1, row);
                    ArraySubCollection<double> iter2 = t2.GetTensorSpan(0, 0, col);

                    double cellValue = 0;

                    for (int i = 0; i < convolutionSize; i++)
                    {
                        cellValue += iter1[i] * iter2[i];
                    }
                }
            }

            return result;
        }
    }
}