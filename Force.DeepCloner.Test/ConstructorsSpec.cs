using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Force.DeepCloner.Tests
{
	[TestClass]
	public class ConstructorsSpec : BaseTest
	{
		public ConstructorsSpec() { }
		public ConstructorsSpec(bool isSafeInit)
			: base(isSafeInit)
		{
		}

		public class T1
		{
			private T1()
			{
			}

			public static T1 Create()
			{
				return new T1();
			}

			public int X { get; set; }
		}

		public class T2
		{
			public T2(int arg1, int arg2)
			{
			}

			public int X { get; set; }
		}

		public class ExClass
		{
			public ExClass()
			{
				throw new Exception();
			}

			public ExClass(string x)
			{
				// does not throw here
			}

			public override bool Equals(object obj)
			{
				throw new Exception();
			}

			public override int GetHashCode()
			{
				throw new Exception();
			}

			public override string ToString()
			{
				throw new Exception();
			}
		}

#if !NETCORE
		public class ClonableClass : ICloneable
		{
			public object X { get; set; }

			public object Clone()
			{
				throw new NotImplementedException();
			}
		}

		[TestMethod]
		public void Cloner_Should_Not_Call_Any_Method_Of_Clonable_Class()
		{
			// just for check, ensure no hidden behaviour in MemberwiseClone
			//Assert.DoesNotThrow(() => new ClonableClass().DeepClone());
			//Assert.DoesNotThrow(() => new { X = new ClonableClass() }.DeepClone());
		}
#endif

		[TestMethod]
		public void Object_With_Private_Constructor_Should_Be_Cloned()
		{
			var t1 = T1.Create();
			t1.X = 42;
			var cloned = t1.DeepClone();
			t1.X = 0;
			Assert.AreEqual(cloned.X, (42));
		}

		[TestMethod]
		public void Object_With_Complex_Constructor_Should_Be_Cloned()
		{
			var t2 = new T2(1, 2);
			t2.X = 42;
			var cloned = t2.DeepClone();
			t2.X = 0;
			Assert.AreEqual(cloned.X, (42));
		}

		[TestMethod]
		public void Anonymous_Object_Should_Be_Cloned()
		{
			var t2 = new { A = 1, B = "x" };
			var cloned = t2.DeepClone();
			Assert.AreEqual(cloned.A, (1));
			Assert.AreEqual(cloned.B, ("x"));
		}

#if !NETCORE
		private class C3 : ContextBoundObject
		{
		}

		private class C4 : MarshalByRefObject
		{
		}

		[TestMethod]
		public void ContextBound_Object_Should_Be_Cloned()
		{
			// FormatterServices.CreateUninitializedObject cannot use context-bound objects
			var c = new C3();
			var cloned = c.DeepClone();
			Assert.AreNotEqual(cloned, null);
		}

		[TestMethod]
		public void MarshalByRef_Object_Should_Be_Cloned()
		{
			// FormatterServices.CreateUninitializedObject cannot use context-bound objects
			var c = new C4();
			var cloned = c.DeepClone();
			Assert.AreNotEqual(cloned, null);
		}
#endif

		[TestMethod]
		public void Cloner_Should_Not_Call_Any_Method_Of_Class_Be_Cloned()
		{
			//Assert.DoesNotThrow(() => new ExClass("x").DeepClone());
			var exClass = new ExClass("x");
			//Assert.DoesNotThrow(() => new[] { exClass, exClass }.DeepClone());
		}
	}
}
