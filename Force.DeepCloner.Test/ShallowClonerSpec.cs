using System;

using Force.DeepCloner.Tests.Objects;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Force.DeepCloner.Tests
{
	[TestClass]
	public class ShallowClonerSpec : BaseTest
	{
		public ShallowClonerSpec() { }
		public ShallowClonerSpec(bool isSafeInit)
			: base(isSafeInit)
		{
		}

		[TestMethod]
		public void SimpleObject_Should_Be_Cloned()
		{
			var obj = new TestObject1 { Int = 42, Byte = 42, Short = 42, Long = 42, DateTime = new DateTime(2001, 01, 01), Char = 'X', Decimal = 1.2m, Double = 1.3, Float = 1.4f, String = "test1", UInt = 42, ULong = 42, UShort = 42, Bool = true, IntPtr = new IntPtr(42), UIntPtr = new UIntPtr(42), Enum = AttributeTargets.Delegate };

			var cloned = obj.ShallowClone();
			Assert.AreEqual(cloned.Byte, (42));
			Assert.AreEqual(cloned.Short, (42));
			Assert.AreEqual(cloned.UShort, (42));
			Assert.AreEqual(cloned.Int, (42));
			Assert.AreEqual(cloned.UInt, ((uint)42));
			Assert.AreEqual(cloned.Long, (42));
			Assert.AreEqual(cloned.ULong, ((ulong)42));
			Assert.AreEqual(cloned.Decimal, ((decimal)1.2));
			Assert.AreEqual(cloned.Double, ((double)1.3));
			Assert.AreEqual(cloned.Float, ((float)1.4f));
			Assert.AreEqual(cloned.Char, ('X'));
			Assert.AreEqual(cloned.String, ("test1"));
			Assert.AreEqual(cloned.DateTime, (new DateTime(2001, 1, 1)));
			Assert.AreEqual(cloned.Bool, (true));
			Assert.AreEqual(cloned.IntPtr, (new IntPtr(42)));
			Assert.AreEqual(cloned.UIntPtr, (new UIntPtr(42)));
			Assert.AreEqual(cloned.Enum, (AttributeTargets.Delegate));
		}

		private class C1
		{
			public object X { get; set; }
		}

		[TestMethod]
		public void Reference_Should_Not_Be_Copied()
		{
			var c1 = new C1();
			c1.X = new object();
			var clone = c1.ShallowClone();
			Assert.AreEqual(clone.X, (c1.X));
		}

		private struct S1 : IDisposable
		{
			public int X;

			public void Dispose()
			{
			}
		}

		[TestMethod]
		public void Struct_Should_Be_Cloned()
		{
			var c1 = new S1();
			c1.X = 1;
			var clone = c1.ShallowClone();
			c1.X = 2;
			Assert.AreEqual(clone.X, (1));
		}

		[TestMethod]
		public void Struct_As_Object_Should_Be_Cloned()
		{
			var c1 = new S1();
			c1.X = 1;
			var clone = (S1)((IDisposable)c1).ShallowClone();
			c1.X = 2;
			Assert.AreEqual(clone.X, (1));
		}

		[TestMethod]
		public void Struct_As_Interface_Should_Be_Cloned()
		{
			var c1 = new DoableStruct1() as IDoable;
			Assert.AreEqual(c1.Do(), (1));
			Assert.AreEqual(c1.Do(), (2));
			var clone = c1.ShallowClone();
			Assert.AreEqual(c1.Do(), (3));
			Assert.AreEqual(clone.Do(), (3));
		}
		
		[TestMethod]
		public void Struct_As_Interface_Should_Be_Cloned_For_DeepClone_Too()
		{
			var c1 = new DoableStruct1() as IDoable;
			Assert.AreEqual(c1.Do(), (1));
			Assert.AreEqual(c1.Do(), (2));
			var clone = c1.DeepClone();
			Assert.AreEqual(c1.Do(), (3));
			Assert.AreEqual(clone.Do(), (3));
		}

		[TestMethod]
		public void Struct_As_Interface_Should_Be_Cloned_In_Object()
		{
			var c1 = new DoableStruct1() as IDoable;
			var t = new Tuple<IDoable>(c1);
			Assert.AreEqual(t.Item1.Do(), (1));
			Assert.AreEqual(t.Item1.Do(), (2));
			var clone = t.ShallowClone();
			Assert.AreEqual(t.Item1.Do(), (3));
			// shallow clone do not copy object
			Assert.AreEqual(clone.Item1.Do(), (4));
		}
		
		[TestMethod]
		public void Struct_As_Interface_Should_Be_Cloned_For_DeepClone_Too_In_Object()
		{
			var c1 = new DoableStruct1() as IDoable;
			var t = new Tuple<IDoable>(c1);
			Assert.AreEqual(t.Item1.Do(), (1));
			Assert.AreEqual(t.Item1.Do(), (2));
			var clone = t.DeepClone();
			Assert.AreEqual(t.Item1.Do(), (3));
			// deep clone copy object
			Assert.AreEqual(clone.Item1.Do(), (3));
		}
		
		[TestMethod]
		public void Primitive_Should_Be_Cloned()
		{
			Assert.AreEqual(((object)null).ShallowClone(), null);
			Assert.AreEqual(3.ShallowClone(), (3));
		}

		[TestMethod]
		public void Array_Should_Be_Cloned()
		{
			var a = new[] { 3, 4 };
			var clone = a.ShallowClone();
			Assert.AreEqual(clone.Length, (2));
			Assert.AreEqual(clone[0], (3));
			Assert.AreEqual(clone[1], (4));
		}
	}
}
