using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Force.DeepCloner.Tests
{
	[TestClass]
	public class GenericsSpec : BaseTest
	{

		public GenericsSpec() { }
		public GenericsSpec(bool isSafeInit)
			: base(isSafeInit)
		{
		}

		[TestMethod]
		public void Tuple_Should_Be_Cloned()
		{
			var c = new Tuple<int, int>(1, 2).DeepClone();
			Assert.AreEqual(c.Item1, (1));
			Assert.AreEqual(c.Item2, (2));

			c = new Tuple<int, int>(1, 2).ShallowClone();
			Assert.AreEqual(c.Item1, (1));
			Assert.AreEqual(c.Item2, (2));
			
			var cc = new Tuple<int, int, int, int, int, int, int>(1, 2, 3, 4, 5, 6, 7).DeepClone();
			Assert.AreEqual(cc.Item7, (7));

			var tuple = new Tuple<int, Generic<object>>(1, new Generic<object>());
			tuple.Item2.Value = tuple;
			var ccc = tuple.DeepClone();
			Assert.AreEqual(ccc, (ccc.Item2.Value));
		}

		[TestMethod]
		public void Generic_Should_Be_Cloned()
		{
			var c = new Generic<int>();
			c.Value = 12;
			Assert.AreEqual(c.DeepClone().Value, (12));

			var c2 = new Generic<object>();
			c2.Value = 12;
			Assert.AreEqual(c2.DeepClone().Value, (12));
		}

		public class C1
		{
			public int X { get; set; }
		}

		public class C2 : C1
		{
			public int Y { get; set; }
		}

		public class Generic<T>
		{
			public T Value { get; set; }
		}

		[TestMethod]
		public void Tuple_Should_Be_Cloned_With_Inheritance_And_Same_Object()
		{
			var c2 = new C2 { X = 1, Y = 2 };
			var c = new Tuple<C1, C2>(c2, c2).DeepClone();
			var cs = new Tuple<C1, C2>(c2, c2).ShallowClone();
			c2.X = 42;
			c2.Y = 42;
			Assert.AreEqual(c.Item1.X, (1));
			Assert.AreEqual(c.Item2.Y, (2));
			Assert.AreEqual(c.Item2, (c.Item1));

			Assert.AreEqual(cs.Item1.X, (42));
			Assert.AreEqual(cs.Item2.Y, (42));
			Assert.AreEqual(cs.Item2, (cs.Item1));
		}
	}
}
