using System;
using System.Collections.Generic;
using System.Text;

using Force.DeepCloner;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Force.DeepCloner.Tests
{
	[TestClass]
	public class ArraysSpec : BaseTest
	{
		public ArraysSpec() {
        }

		public ArraysSpec(object isSafeInit)
			: base((bool)isSafeInit)
		{
		}

        [TestMethod]
		public void TestSimpleEqual()
		{
			Assert.AreEqual(0, 0);
		}

		[TestMethod]
		public void IntArray_Should_Be_Cloned()
		{
			var arr = new[] { 1, 2, 3 };
			var cloned = arr.DeepClone();
			Assert.AreEqual(cloned.Length, (3));
			CollectionAssert.AreEqual(arr, cloned);
		}

		[TestMethod]
		public void StringArray_Should_Be_Cloned()
		{
			var arr = new[] { "1", "2", "3" };
			var cloned = arr.DeepClone();
			Assert.AreEqual(cloned.Length, (3));
			CollectionAssert.AreEqual(arr, cloned);
		}

		[TestMethod]
		public void StringArray_Should_Be_Cloned_Two_Arrays()
		{
			// checking that cached object correctly clones arrays of different length
			var arr = new[] { "111111111111111111111", "2", "3" };
			var cloned = arr.DeepClone();
			Assert.AreEqual(cloned.Length, (3));
			CollectionAssert.AreEqual(arr, cloned);
			// strings should not be copied
			Assert.AreEqual(ReferenceEquals(arr[1], cloned[1]), true);

			arr = new[] { "1", "2", "3", "4" };
			cloned = arr.DeepClone();
			Assert.AreEqual(cloned.Length, (4));
			CollectionAssert.AreEqual(arr, cloned);

			arr = new string[0];
			cloned = arr.DeepClone();
			Assert.AreEqual(cloned.Length, (0));

			if (1.Equals(1)) arr = null;
			Assert.AreEqual(arr.DeepClone(), null);
		}

		[TestMethod]
		public void StringArray_Casted_As_Object_Should_Be_Cloned()
		{
			// checking that cached object correctly clones arrays of different length
			var arr = (object)new[] { "1", "2", "3" };
			var cloned = arr.DeepClone() as string[];
			Assert.AreEqual(cloned.Length, (3));
			CollectionAssert.AreEqual((string[])arr, cloned);
			// strings should not be copied
			Assert.AreEqual(ReferenceEquals(((string[])arr)[1], cloned[1]), true);
		}

		[TestMethod]
		public void ByteArray_Should_Be_Cloned()
		{
			// checking that cached object correctly clones arrays of different length
			var arr = Encoding.ASCII.GetBytes("test");
			var cloned = arr.DeepClone();
			CollectionAssert.AreEqual(arr, cloned);

			arr = Encoding.ASCII.GetBytes("test testtest testtest testtest testtest testtest testtest testtest testtest testtest testtest testtest testtest testte");
			cloned = arr.DeepClone();
			CollectionAssert.AreEqual(arr, cloned);
		}

		public class C1
		{
			public C1(int x)
			{
				X = x;
			}

			public int X { get; set; }
		}

		[TestMethod]
		public void ClassArray_Should_Be_Cloned()
		{
			var arr = new[] { new C1(1), new C1(2) };
			var cloned = arr.DeepClone();
			Assert.AreEqual(cloned.Length, (2));
			Assert.AreEqual(cloned[0].X, (1));
			Assert.AreEqual(cloned[1].X, (2));
			Assert.AreNotEqual(cloned[0], (arr[0]));
			Assert.AreNotEqual(cloned[1], (arr[1]));
		}

		public struct S1
		{
			public S1(int x)
			{
				X = x;
			}

			public int X;
		}

		public struct S2
		{
			public C1 C;
		}

		[TestMethod]
		public void StructArray_Should_Be_Cloned()
		{
			var arr = new[] { new S1(1), new S1(2) };
			var cloned = arr.DeepClone();
			Assert.AreEqual(cloned.Length, (2));
			Assert.AreEqual(cloned[0].X, (1));
			Assert.AreEqual(cloned[1].X, (2));
		}

		[TestMethod]
		public void StructArray_With_Class_Should_Be_Cloned()
		{
			var arr = new[] { new S2 { C = new C1(1) }, new S2 { C = new C1(2) } };
			var cloned = arr.DeepClone();
			Assert.AreEqual(cloned.Length, (2));
			Assert.AreEqual(cloned[0].C.X, (1));
			Assert.AreEqual(cloned[1].C.X, (2));
			Assert.AreNotEqual(cloned[0].C, (arr[0].C));
			Assert.AreNotEqual(cloned[1].C, (arr[1].C));
		}

		[TestMethod]
		public void NullArray_hould_Be_Cloned()
		{
			var arr = new C1[] { null, null };
			var cloned = arr.DeepClone();
			Assert.AreEqual(cloned.Length, (2));
			Assert.AreEqual(cloned[0], null);
			Assert.AreEqual(cloned[1], null);
		}

		[TestMethod]
		public void NullAsArray_hould_Be_Cloned()
		{
			var arr = (int[])null;
// ReSharper disable ExpressionIsAlwaysNull
			var cloned = arr.DeepClone();
// ReSharper restore ExpressionIsAlwaysNull
			Assert.AreEqual(cloned, null);
		}

		[TestMethod]
		public void IntList_Should_Be_Cloned()
		{
			// TODO: better performance for this type
			var arr = new List<int> { 1, 2, 3 };
			var cloned = arr.DeepClone();
			Assert.AreEqual(cloned.Count, (3));
			Assert.AreEqual(cloned[0], (1));
			Assert.AreEqual(cloned[1], (2));
			Assert.AreEqual(cloned[2], (3));
		}

		[TestMethod]
		public void Dictionary_Should_Be_Cloned()
		{
			// TODO: better performance for this type
			var d = new Dictionary<string, decimal>();
			d["a"] = 1;
			d["b"] = 2;
			var cloned = d.DeepClone();
			Assert.AreEqual(cloned.Count, (2));
			Assert.AreEqual(cloned["a"], (1));
			Assert.AreEqual(cloned["b"], (2));
		}

		[TestMethod]
		public void Array_Of_Same_Arrays_Should_Be_Cloned()
		{
			var c1 = new[] { 1, 2, 3 };
			var arr = new[] { c1, c1, c1, c1, c1 };
			var cloned = arr.DeepClone();

			Assert.AreEqual(cloned.Length, (5));
			// lot of objects for checking reference dictionary optimization
			Assert.AreEqual(ReferenceEquals(arr[0], cloned[0]), false);
			Assert.AreEqual(ReferenceEquals(cloned[0], cloned[1]), true);
			Assert.AreEqual(ReferenceEquals(cloned[1], cloned[2]), true);
			Assert.AreEqual(ReferenceEquals(cloned[1], cloned[3]), true);
			Assert.AreEqual(ReferenceEquals(cloned[1], cloned[4]), true);
		}

		public class AC
		{
			public int[] A { get; set; }

			public int[] B { get; set; }
		}

		[TestMethod]
		public void Class_With_Same_Arrays_Should_Be_Cloned()
		{
			var ac = new AC();
			ac.A = ac.B = new int[3];
			var clone = ac.DeepClone();
			Assert.AreEqual(ReferenceEquals(ac.A, clone.A), false);
			Assert.AreEqual(ReferenceEquals(clone.A, clone.B), true);
		}

		[TestMethod]
		public void Class_With_Null_Array_hould_Be_Cloned()
		{
			var ac = new AC();
			var cloned = ac.DeepClone();
			Assert.AreEqual(cloned.A, null);
			Assert.AreEqual(cloned.B, null);
		}

		[TestMethod]
		public void MultiDim_Array_Should_Be_Cloned()
		{
			var arr = new int[2, 2];
			arr[0, 0] = 1;
			arr[0, 1] = 2;
			arr[1, 0] = 3;
			arr[1, 1] = 4;
			var clone = arr.DeepClone();
			Assert.AreEqual(ReferenceEquals(arr, clone), false);
			Assert.AreEqual(clone[0, 0], (1));
			Assert.AreEqual(clone[0, 1], (2));
			Assert.AreEqual(clone[1, 0], (3));
			Assert.AreEqual(clone[1, 1], (4));
		}

		[TestMethod]
		public void MultiDim_Array_Should_Be_Cloned2()
		{
			var arr = new int[2, 2, 1];
			arr[0, 0, 0] = 1;
			arr[0, 1, 0] = 2;
			arr[1, 0, 0] = 3;
			arr[1, 1, 0] = 4;
			var clone = arr.DeepClone();
			Assert.AreEqual(ReferenceEquals(arr, clone), false);
			Assert.AreEqual(clone[0, 0, 0], (1));
			Assert.AreEqual(clone[0, 1, 0], (2));
			Assert.AreEqual(clone[1, 0, 0], (3));
			Assert.AreEqual(clone[1, 1, 0], (4));
		}

		[TestMethod]
		public void MultiDim_Array_Of_Classes_Should_Be_Cloned()
		{
			var arr = new AC[2, 2];
			arr[0, 0] = arr[1, 1] = new AC();
			var clone = arr.DeepClone();
			Assert.AreNotEqual(clone[0, 0], null);
			Assert.AreNotEqual(clone[1, 1], null);
			Assert.AreEqual(clone[1, 1], (clone[0, 0]));
			Assert.AreNotEqual(clone[1, 1], (arr[0, 0]));
		}

		[TestMethod]
		public void NonZero_Based_Array_Should_Be_Cloned()
		{
			var arr = Array.CreateInstance(typeof(int), new[] { 2 }, new[] { 1 });
			
			arr.SetValue(1, 1);
			arr.SetValue(2, 2);
			var clone = arr.DeepClone();
			Assert.AreEqual(clone.GetValue(1), (1));
			Assert.AreEqual(clone.GetValue(2), (2));
		}

		[TestMethod]
		public void NonZero_Based_MultiDim_Array_Should_Be_Cloned()
		{
			var arr = Array.CreateInstance(typeof(int), new[] { 2, 2 }, new[] { 1, 1 });

			arr.SetValue(1, 1, 1);
			arr.SetValue(2, 2, 2);
			var clone = arr.DeepClone();
			Assert.AreEqual(clone.GetValue(1, 1), (1));
			Assert.AreEqual(clone.GetValue(2, 2), (2));
		}

		[TestMethod]
		public void Array_As_Generic_Array_Should_Be_Cloned()
		{
			var arr = new[] { 1, 2, 3 };
			var genArr = (Array)arr;
			var clone = (int[])genArr.DeepClone();
			Assert.AreEqual(clone.Length, (3));
			Assert.AreEqual(clone[0], (1));
			Assert.AreEqual(clone[1], (2));
			Assert.AreEqual(clone[2], (3));
		}

		[TestMethod]
		public void Array_As_IEnumerable_Should_Be_Cloned()
		{
			var arr = new[] { 1, 2, 3 };
			var genArr = (IEnumerable<int>)arr;
			var clone = (int[])genArr.DeepClone();
// ReSharper disable PossibleMultipleEnumeration
			Assert.AreEqual(clone.Length, (3));
			Assert.AreEqual(clone[0], (1));
			Assert.AreEqual(clone[1], (2));
			Assert.AreEqual(clone[2], (3));
			// ReSharper restore PossibleMultipleEnumeration
		}
	}
}
