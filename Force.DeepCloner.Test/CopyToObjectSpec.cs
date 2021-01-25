using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Force.DeepCloner.Tests
{
	[TestClass]
	public class CopyToObjectSpec
	{
		public class C1
		{
			public int A { get; set; }

			public virtual string B { get; set; }

			public byte[] C { get; set; }
		}

		public class C2 : C1
		{
			public decimal D { get; set; }

			public new int A { get; set; }
		}

		public class C4 : C1
		{
		}

		public class C3
		{
			public C1 A { get; set; }

			public C1 B { get; set; }
		}

		public interface I1
		{
			int A { get; set; }
		}

		public struct S1 : I1
		{
			public int A { get; set; }
		}

		[TestMethod]
		public void Simple_Class_Should_Be_Cloned_Deep()
		{
			var cFrom = new C1
			{
				A = 12,
				B = "testestest",
				C = new byte[] { 1, 2, 3 }
			};

			var cTo = new C1
			{
				A = 11,
				B = "tes",
				C = new byte[] { 1 }
			};

			var cToRef = cTo;

            cFrom.DeepCloneTo(cTo);

			Assert.AreEqual(ReferenceEquals(cTo, cToRef), true);
			Assert.AreEqual(cTo.A, (12));
			Assert.AreEqual(cTo.B, ("testestest"));
			Assert.AreEqual(cTo.C.Length, (3));
			Assert.AreEqual(cTo.C[2], (3));
		}

        [TestMethod]
		public void Simple_Class_Should_Be_Cloned_Shallow()
		{
			var cFrom = new C1
			{
				A = 12,
				B = "testestest",
				C = new byte[] { 1, 2, 3 }
			};

			var cTo = new C1
			{
				A = 11,
				B = "tes",
				C = new byte[] { 1 }
			};

			var cToRef = cTo;

            cFrom.ShallowCloneTo(cTo);

			Assert.AreEqual(ReferenceEquals(cTo, cToRef), true);
			Assert.AreEqual(cTo.A, (12));
			Assert.AreEqual(cTo.B, ("testestest"));
			Assert.AreEqual(cTo.C.Length, (3));
			Assert.AreEqual(cTo.C[2], (3));
		}


		[TestMethod]
		public void Descendant_Class_Should_Be_Cloned_Deep()
		{
			var cFrom = new C1
			{
				A = 12,
				B = "testestest",
				C = new byte[] { 1, 2, 3 }
			};

			var cTo = new C2
			{
				A = 11,
				D = 42.3m
			};

			var cToRef = cTo;

            cFrom.DeepCloneTo(cTo);

			Assert.AreEqual(ReferenceEquals(cTo, cToRef), true);
			Assert.AreEqual(cTo.A, (11));
			Assert.AreEqual(((C1)cTo).A, (12));
			Assert.AreEqual(cTo.D, (42.3m));
		}

        [TestMethod]
		public void Descendant_Class_Should_Be_Cloned_Shallow()
		{
			var cFrom = new C1
			{
				A = 12,
				B = "testestest",
				C = new byte[] { 1, 2, 3 }
			};

			var cTo = new C2
			{
				A = 11,
				D = 42.3m
			};

			var cToRef = cTo;

            cFrom.ShallowCloneTo(cTo);

			Assert.AreEqual(ReferenceEquals(cTo, cToRef), true);
			Assert.AreEqual(cTo.A, (11));
			Assert.AreEqual(((C1)cTo).A, (12));
			Assert.AreEqual(cTo.D, (42.3m));
		}



		[TestMethod]
		public void Class_With_Subclass_Should_Be_Shallow_CLoned()
		{
			var c1 = new C1 { A = 12 };
			var cFrom = new C3 { A = c1, B = c1 };
			var cTo = cFrom.ShallowCloneTo(new C3());
			Assert.AreEqual(ReferenceEquals(cFrom.A, cTo.A), true);
			Assert.AreEqual(ReferenceEquals(cFrom.B, cTo.B), true);
			Assert.AreEqual(ReferenceEquals(cTo.A, cTo.B), true);
		}

		[TestMethod]
		public void Class_With_Subclass_Should_Be_Deep_CLoned()
		{
			var c1 = new C1 { A = 12 };
			var cFrom = new C3 { A = c1, B = c1 };
			var cTo = cFrom.DeepCloneTo(new C3());
			Assert.AreEqual(ReferenceEquals(cFrom.A, cTo.A), false);
			Assert.AreEqual(ReferenceEquals(cFrom.B, cTo.B), false);
			Assert.AreEqual(ReferenceEquals(cTo.A, cTo.B), true);
		}

		[TestMethod]
		public void Copy_To_Null_Should_Return_Null_Deep()
		{
			var c1 = new C1();
            Assert.AreEqual(c1.DeepCloneTo((C1)null), null);
		}

        [TestMethod]
		public void Copy_To_Null_Should_Return_Null_Shallow()
		{
			var c1 = new C1();
            Assert.AreEqual(c1.ShallowCloneTo((C1)null), null);
		}

		[TestMethod]
		public void Copy_From_Null_Should_Throw_Error_Deep()
		{
			C1 c1 = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(() => c1.DeepCloneTo(new C1()));
		}

        [TestMethod]
		public void Copy_From_Null_Should_Throw_Error_Shallow()
		{
			C1 c1 = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<ArgumentNullException>(() => c1.ShallowCloneTo(new C1()));
		}

		[TestMethod]
		public void Invalid_Inheritance_Should_Throw_Error_Deep()
		{
			C1 c1 = new C4();
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<InvalidOperationException>(() => c1.DeepCloneTo(new C2()));
		}

        [TestMethod]
		public void Invalid_Inheritance_Should_Throw_Error_Shallow()
		{
			C1 c1 = new C4();
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<InvalidOperationException>(() => c1.ShallowCloneTo(new C2()));
		}

		[TestMethod]
		public void Struct_As_Interface_ShouldNot_Be_Cloned_Deep()
		{
			S1 sFrom = new S1 { A = 42 };
			S1 sTo = new S1();
			var objTo = (I1)sTo;
			objTo.A = 23;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<InvalidOperationException>(() => ((I1)sFrom).DeepCloneTo(objTo));
		}

        [TestMethod]
		public void Struct_As_Interface_ShouldNot_Be_Cloned_Shallow()
		{
			S1 sFrom = new S1 { A = 42 };
			S1 sTo = new S1();
			var objTo = (I1)sTo;
			objTo.A = 23;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.ThrowsException<InvalidOperationException>(() => ((I1)sFrom).ShallowCloneTo(objTo));
		}

		[TestMethod]
		public void String_Should_Not_Be_Cloned()
		{
			var s1 = "abc";
			var s2 = "def";
			Assert.ThrowsException<InvalidOperationException>(() => s1.ShallowCloneTo(s2));
		}

		[TestMethod]
		public void Array_Should_Be_Cloned_Correct_Size_Deep()
		{
			var arrFrom = new[] { 1, 2, 3 };
			var arrTo = new[] { 4, 5, 6 };
			arrFrom.DeepCloneTo(arrTo);
			Assert.AreEqual(arrTo.Length, (3));
			Assert.AreEqual(arrTo[0], (1));
			Assert.AreEqual(arrTo[1], (2));
			Assert.AreEqual(arrTo[2], (3));
		}

        [TestMethod]
		public void Array_Should_Be_Cloned_Correct_Size_Shallow()
		{
			var arrFrom = new[] { 1, 2, 3 };
			var arrTo = new[] { 4, 5, 6 };
			arrFrom.ShallowCloneTo(arrTo);
			Assert.AreEqual(arrTo.Length, (3));
			Assert.AreEqual(arrTo[0], (1));
			Assert.AreEqual(arrTo[1], (2));
			Assert.AreEqual(arrTo[2], (3));
		}


		[TestMethod]
		public void Array_Should_Be_Cloned_From_Is_Bigger_Deep()
		{
			var arrFrom = new[] { 1, 2, 3 };
			var arrTo = new[] { 4, 5 };
			arrFrom.DeepCloneTo(arrTo);
			Assert.AreEqual(arrTo.Length, (2));
			Assert.AreEqual(arrTo[0], (1));
			Assert.AreEqual(arrTo[1], (2));
		}

        [TestMethod]
		public void Array_Should_Be_Cloned_From_Is_Bigger_Shallow()
		{
			var arrFrom = new[] { 1, 2, 3 };
			var arrTo = new[] { 4, 5 };
			arrFrom.ShallowCloneTo(arrTo);
			Assert.AreEqual(arrTo.Length, (2));
			Assert.AreEqual(arrTo[0], (1));
			Assert.AreEqual(arrTo[1], (2));
		}

		[TestMethod]
		public void Array_Should_Be_Cloned_From_Is_Smaller_Deep()
		{
			var arrFrom = new[] { 1, 2 };
			var arrTo = new[] { 4, 5, 6 };
			arrFrom.DeepCloneTo(arrTo);
			Assert.AreEqual(arrTo.Length, (3));
			Assert.AreEqual(arrTo[0], (1));
			Assert.AreEqual(arrTo[1], (2));
			Assert.AreEqual(arrTo[2], (6));
		}

        [TestMethod]
		public void Array_Should_Be_Cloned_From_Is_Smaller_Shallow()
		{
			var arrFrom = new[] { 1, 2 };
			var arrTo = new[] { 4, 5, 6 };
			arrFrom.ShallowCloneTo(arrTo);
			Assert.AreEqual(arrTo.Length, (3));
			Assert.AreEqual(arrTo[0], (1));
			Assert.AreEqual(arrTo[1], (2));
			Assert.AreEqual(arrTo[2], (6));
		}

		[TestMethod]
		public void Shallow_Array_Should_Be_Cloned()
		{
			var c1 = new C1();
			var arrFrom = new[] { c1, c1, c1 };
			var arrTo = new C1[4];
			arrFrom.ShallowCloneTo(arrTo);
			Assert.AreEqual(arrTo.Length, (4));
			Assert.AreEqual(arrTo[0], (c1));
			Assert.AreEqual(arrTo[1], (c1));
			Assert.AreEqual(arrTo[2], (c1));
			Assert.AreEqual(arrTo[3], null);
		}

		[TestMethod]
		public void Deep_Array_Should_Be_Cloned()
		{
			var c1 = new C4();
			var c3 = new C3 { A = c1, B = c1 };
			var arrFrom = new[] { c3, c3, c3 };
			var arrTo = new C3[4];
			arrFrom.DeepCloneTo(arrTo);
			Assert.AreEqual(arrTo.Length, (4));
			Assert.AreNotEqual(arrTo[0], (c1));
			Assert.AreEqual(arrTo[0], (arrTo[1]));
			Assert.AreEqual(arrTo[1], (arrTo[2]));
			Assert.AreNotEqual(arrTo[2].A, (c1));
			Assert.AreEqual(arrTo[2].A, (arrTo[2].B));
			Assert.AreEqual(arrTo[3], null);
		}

		[TestMethod]
		public void Non_Zero_Based_Array_Should_Be_Cloned_Deep()
		{
			var arrFrom = Array.CreateInstance(typeof(int), new[] { 2 }, new[] { 1 });
			// with offset. its ok
			var arrTo = Array.CreateInstance(typeof(int), new[] { 2 }, new[] { 0 });
			arrFrom.SetValue(1, 1);
			arrFrom.SetValue(2, 2);
			arrFrom.DeepCloneTo(arrTo);
			Assert.AreEqual(arrTo.Length, (2));
			Assert.AreEqual(arrTo.GetValue(0), (1));
			Assert.AreEqual(arrTo.GetValue(1), (2));
		}

        [TestMethod]
		public void Non_Zero_Based_Array_Should_Be_Cloned_Shallow()
		{
			var arrFrom = Array.CreateInstance(typeof(int), new[] { 2 }, new[] { 1 });
			// with offset. its ok
			var arrTo = Array.CreateInstance(typeof(int), new[] { 2 }, new[] { 0 });
			arrFrom.SetValue(1, 1);
			arrFrom.SetValue(2, 2);
			arrFrom.ShallowCloneTo(arrTo);
			Assert.AreEqual(arrTo.Length, (2));
			Assert.AreEqual(arrTo.GetValue(0), (1));
			Assert.AreEqual(arrTo.GetValue(1), (2));
		}

		[TestMethod]
		public void MultiDim_Array_Should_Be_Cloned_Deep()
		{
			var arrFrom = Array.CreateInstance(typeof(int), new[] { 2, 2 }, new[] { 1, 1 });
			// with offset. its ok
			var arrTo = Array.CreateInstance(typeof(int), new[] { 1, 1 }, new[] { 0, 0 });
			arrFrom.SetValue(1, 1, 1);
			arrFrom.SetValue(2, 2, 2);
			arrFrom.DeepCloneTo(arrTo);
			Assert.AreEqual(arrTo.Length, (1));
			Assert.AreEqual(arrTo.GetValue(0, 0), (1));
		}

        [TestMethod]
		public void MultiDim_Array_Should_Be_Cloned_Shallow()
		{
			var arrFrom = Array.CreateInstance(typeof(int), new[] { 2, 2 }, new[] { 1, 1 });
			// with offset. its ok
			var arrTo = Array.CreateInstance(typeof(int), new[] { 1, 1 }, new[] { 0, 0 });
			arrFrom.SetValue(1, 1, 1);
			arrFrom.SetValue(2, 2, 2);
			arrFrom.ShallowCloneTo(arrTo);
			Assert.AreEqual(arrTo.Length, (1));
			Assert.AreEqual(arrTo.GetValue(0, 0), (1));
		}

		[TestMethod]
		public void TwoDim_Array_Should_Be_Cloned_Deep()
		{
			var arrFrom = new[,] { { 1, 2 }, { 3, 4 } };
			// with offset. its ok
			var arrTo = new int[3, 1];
			arrFrom.DeepCloneTo(arrTo);
			Assert.AreEqual(arrTo[0, 0], (1));
			Assert.AreEqual(arrTo[1, 0], (3));

			arrTo = new int[2, 2];
			arrFrom.DeepCloneTo(arrTo);
			Assert.AreEqual(arrTo[0, 0], (1));
			Assert.AreEqual(arrTo[0, 1], (2));
			Assert.AreEqual(arrTo[1, 0], (3));
		}

        [TestMethod]
		public void TwoDim_Array_Should_Be_Cloned_Shallow()
		{
			var arrFrom = new[,] { { 1, 2 }, { 3, 4 } };
			// with offset. its ok
			var arrTo = new int[3, 1];
			arrFrom.ShallowCloneTo(arrTo);
			Assert.AreEqual(arrTo[0, 0], (1));
			Assert.AreEqual(arrTo[1, 0], (3));

			arrTo = new int[2, 2];
			arrFrom.ShallowCloneTo(arrTo);
			Assert.AreEqual(arrTo[0, 0], (1));
			Assert.AreEqual(arrTo[0, 1], (2));
			Assert.AreEqual(arrTo[1, 0], (3));
		}


	    [TestMethod]
	    public void Shallow_Clone_Of_MultiDim_Array_Should_Not_Perform_Deep()
	    {
	        var c1 = new C1();
	        var arrFrom = new[,] { { c1, c1 }, { c1, c1 } };
	        // with offset. its ok
	        var arrTo = new C1[3, 1];
	        arrFrom.ShallowCloneTo(arrTo);
	        Assert.AreEqual(ReferenceEquals(c1, arrTo[0, 0]), true);
	        Assert.AreEqual(ReferenceEquals(c1, arrTo[1, 0]), true);

	        var arrFrom2 = new C1[1, 1, 1];
	        arrFrom2[0, 0, 0] = c1;
	        var arrTo2 = new C1[1, 1, 1];
	        arrFrom2.ShallowCloneTo(arrTo2);
	        Assert.AreEqual(ReferenceEquals(c1, arrTo2[0, 0, 0]), true);
	    }

	    [TestMethod]
	    public void Deep_Clone_Of_MultiDim_Array_Should_Perform_Deep()
	    {
	        var c1 = new C1();
	        var arrFrom = new[,] { { c1, c1 }, { c1, c1 } };
	        // with offset. its ok
	        var arrTo = new C1[3, 1];
	        arrFrom.DeepCloneTo(arrTo);
	        Assert.AreEqual(ReferenceEquals(c1, arrTo[0, 0]), false);
	        Assert.AreEqual(ReferenceEquals(arrTo[0, 0], arrTo[1, 0]), true);

	        var arrFrom2 = new C1[1, 1, 2];
	        arrFrom2[0, 0, 0] = c1;
	        arrFrom2[0, 0, 1] = c1;
	        var arrTo2 = new C1[1, 1, 2];
	        arrFrom2.DeepCloneTo(arrTo2);
	        Assert.AreEqual(ReferenceEquals(c1, arrTo2[0, 0, 0]), false);
	        Assert.AreEqual(ReferenceEquals(arrTo2[0, 0, 1], arrTo2[0, 0, 0]), true);
	    }

		[TestMethod]
		public void Dictionary_Should_Be_Deeply_Cloned()
		{
			var d1 = new Dictionary<string, string>{ { "A", "B" }, { "C", "D" } };
			var d2 = new Dictionary<string, string>();
			d1.DeepCloneTo(d2);
			d1["A"] = "E";
			Assert.AreEqual(d2.Count, (2));
			Assert.AreEqual(d2["A"], ("B"));
			Assert.AreEqual(d2["C"], ("D"));

			// big dictionary
			d1.Clear();
			for (var i = 0; i < 1000; i++)
				d1[i.ToString()] = i.ToString();
			d1.DeepCloneTo(d2);
			Assert.AreEqual(d2.Count, (1000));
			Assert.AreEqual(d2["557"], ("557"));
		}

		public class D1
		{
			public int A { get; set; }
		}

		public class D2 : D1
		{
			public int B { get; set; }

			public D2(D1 d1)
			{
				B = 14;
				d1.DeepCloneTo(this);
			}
		}

		[TestMethod]
		public void Inner_Implementation_In_Class_Should_Work()
		{
			var baseObject = new D1 { A = 12 };
			var wrapper = new D2(baseObject);
			Assert.AreEqual(wrapper.A, (12));
			Assert.AreEqual(wrapper.B, (14));
		}
	}
}