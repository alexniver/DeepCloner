using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Force.DeepCloner.Tests
{
	[TestClass]
	public class LoopCheckSpec : BaseTest
	{
		public LoopCheckSpec() { }
		public LoopCheckSpec(bool isSafeInit)
			: base(isSafeInit)
		{
		}

		public class C1
		{
			public int F { get; set; }

			public C1 A { get; set; }
		}

		[TestMethod]
		public void SimpleLoop_Should_Be_Handled()
		{
			var c1 = new C1();
			var c2 = new C1();
			c1.F = 1;
			c2.F = 2;
			c1.A = c2;
			c1.A.A = c1;
			var cloned = c1.DeepClone();

			Assert.AreNotEqual(cloned.A, null);
			Assert.AreEqual(cloned.A.A.F, (cloned.F));
			Assert.AreEqual(cloned.A.A, (cloned));
		}

		[TestMethod]
		public void Object_Own_Loop_Should_Be_Handled()
		{
			var c1 = new C1();
			c1.F = 1;
			c1.A = c1;
			var cloned = c1.DeepClone();

			Assert.AreNotEqual(cloned.A, null);
			Assert.AreEqual(cloned.A.F, (cloned.F));
			Assert.AreEqual(cloned.A, (cloned));
		}

		[TestMethod]
		public void Array_Of_Same_Objects_Should_Be_Cloned()
		{
			var c1 = new C1();
			var arr = new[] { c1, c1, c1 };
			c1.F = 1;
			var cloned = arr.DeepClone();

			Assert.AreEqual(cloned.Length, (3));
			Assert.AreEqual(cloned[0], (cloned[1]));
			Assert.AreEqual(cloned[1], (cloned[2]));
		}
	}
}
