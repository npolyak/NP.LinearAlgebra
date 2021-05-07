using System;

namespace MainTest
{
    public static class SubTensorIterationTest
    {
        public static void Test()
        {
            Tensor<int> tensor = THelper.Arange(0, 30).Reshape(2, 3, 5);

            tensor.TheDimensionPermutation = new Permutation(2, 1, 0);

            foreach (Tensor<int> subTensor in tensor)
            {
                foreach (Tensor<int> subSubTensor in subTensor)
                {
                    foreach (Tensor<int> subSubSubTensor in subSubTensor)
                    {
                        Console.WriteLine(subSubSubTensor.TensorStartValue);
                    }
                }
            }
        }
    }
}
