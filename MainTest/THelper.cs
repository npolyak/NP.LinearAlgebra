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

        public static Tensor<T> Create<T>(this IEnumerable<T> collection, IEnumerable<int> dimensions = null) =>
            new Tensor<T>(collection);


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

        public static Tensor<TOut> Cast<TIn, TOut>(this Tensor<TIn> t, Func<TIn, TOut> converter = null)
        {
            if (converter == null)
            {
                converter = NumericUtils.GetNumericConverter<TIn, TOut>();
            }

            Tensor<TOut> result = new Tensor<TOut>(t.ShapeDimensions.ToArray());

            result.SetSpan(t.TheSpan.ToArray().Select(item => converter(item)).ToArray());

            return result;
        }


        public static void DoCellByCell<TTarget, TSource>
        (
            this Tensor<TTarget> target,
            TSource source,
            RefFunc<TTarget, TSource> operation)
        {
            for (int i = 0; i < target.TheSpan.Length; i++)
            {
                operation(ref target.TheSpan[i], source);
            }
        }


        public static void DoCellByCell<TTarget, TSource>
        (
            this Tensor<TTarget> target, 
            Tensor<TSource> source, 
            RefFunc<TTarget, TSource> operation)
        {
            target.CheckEqualDimensions(source);

            for (int i = 0; i < target.TheSpan.Length; i++)
            {
                operation(ref target.TheSpan[i], source.TheSpan[i]);
            }
        }

        public static Tensor<TOut> DoCellByCell<TOut, TIn1, TIn2>(this Tensor<TIn1> t1, Tensor<TIn2> t2, Func<TIn1, TIn2, TOut> operation)
        {
            t1.CheckEqualDimensions(t2);

            Tensor<TOut> result = new Tensor<TOut>(t1.ShapeDimensions.ToArray());

            for(int i = 0; i < t1.TheSpan.Length; i++)
            {
                result.TheSpan[i] = operation(t1.TheSpan[i], t2.TheSpan[i]);
            }

            return result;
        }

        public static Tensor<TOut> DoScalarCellByCell<TOut, TIn1, TIn2>(this Tensor<TIn1> t1, TIn2 val2, Func<TIn1, TIn2, TOut> operation)
        {
            Tensor<TOut> result = new Tensor<TOut>(t1.ShapeDimensions.ToArray());

            for (int i = 0; i < t1.TheSpan.Length; i++)
            {
                result.TheSpan[i] = operation(t1.TheSpan[i], val2);
            }

            return result;
        }

        public static Tensor<double> DoDoubleScalarCellByCell<TIn1, TIn2>(this Tensor<TIn1> t1, TIn2 val2, Func<double, double, double> operation)
        {
            var converter1 = NumericUtils.GetConverterToDouble<TIn1>();
            var converter2 = NumericUtils.GetConverterToDouble<TIn2>();

            double dVal2 = converter2(val2);

            return t1.DoScalarCellByCell(val2, (TIn1 item1, TIn2 item2) => operation(converter1(item1), dVal2));
        }

        public static Tensor<int> DoIntScalarCellByCell<TIn1, TIn2>(this Tensor<TIn1> t1, TIn2 val2, Func<int, int, int> operation)
        {
            var converter1 = NumericUtils.GetNumericConverter<TIn1, int>();
            var converter2 = NumericUtils.GetNumericConverter<TIn2, int>();

            int dVal2 = converter2(val2);

            return t1.DoScalarCellByCell(val2, (TIn1 item1, TIn2 item2) => operation(converter1(item1), dVal2));
        }

        public static Tensor<int> DoIntCellByCell<TIn1, TIn2>(this Tensor<TIn1> t1, Tensor<TIn2> t2, Func<int, int, int> operation)
        {
            var converter1 = NumericUtils.GetNumericConverter<TIn1, int>();
            var converter2 = NumericUtils.GetNumericConverter<TIn2, int>();

            return t1.DoCellByCell(t2, (TIn1 item1, TIn2 item2) => operation(converter1(item1), converter2(item2)));
        }

        public static Tensor<double> DoDoubleCellByCell<TIn1, TIn2>(this Tensor<TIn1> t1, Tensor<TIn2> t2, Func<double, double, double> operation)
        {
            var converter1 = NumericUtils.GetConverterToDouble<TIn1>();
            var converter2 = NumericUtils.GetConverterToDouble<TIn2>();

            return t1.DoCellByCell(t2, (TIn1 item1, TIn2 item2) => operation(converter1(item1), converter2(item2)));
        }
    }
}