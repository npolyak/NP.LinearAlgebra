using MainTest;
using NP.Utilities;
using System;

//int[] i1 = { 5, 1, 2, 3};
//int[] i2 = { 5, 1, 2, 3};

//if (i1.EqualCollections(i2))
//{
//    Console.WriteLine("Collections Equal");
//}
//else
//{
//    Console.WriteLine("Collections are not Equal"); 
//}

//Tensor<int> tensor = THelper.Arange(0, 30);

//tensor = tensor.Reshape(new[] { 2, 3, 5 });

//string str1 = tensor.ShapeDimensions.ToStr();
//Console.WriteLine(str1);

//Console.WriteLine(tensor.ShapeDimensionSizes.ToStr());

////Console.WriteLine(tensor.Get(2, 1, 4));

//Console.WriteLine("\n\n");

//Console.WriteLine(tensor.ToFullStr());

//Tensor<int> t = THelper.Create(1, new[] { 20 });

//Console.WriteLine(t.ToFullStr());

//t = THelper.Arange(0, 10000).Reshape(100, 100);

//Console.Write(t.ToFullStr());

Range r = 2..3;

//Console.WriteLine(r);

//Console.WriteLine(tensor.ShapeDimensionSizes.ToStr());

////Console.WriteLine(tensor.ToString());

Tensor<int> tensor1 = THelper.Arange(0, 30);

tensor1 = tensor1.Reshape(2, 3, 5);

var t2 = (tensor1 + tensor1).Cast<double, int>();//.Convert<double, int>(d => (int) d);

Console.WriteLine(t2.ToFullStr());