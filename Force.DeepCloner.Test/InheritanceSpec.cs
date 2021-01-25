using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Force.DeepCloner.Tests
{
	[TestClass]
	public class InheritanceSpec : BaseTest
	{
		public InheritanceSpec() { }

		public InheritanceSpec(bool isSafeInit)
			: base(isSafeInit)
		{
		}

		public class C1 : IDisposable
		{
			[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
			public int X;

			[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
			public int Y;

			[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
			public object O; // make it not safe

			public void Dispose()
			{
			}
		}

		public class C2 : C1
		{
			[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
			public new int X;

			[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
			public int Z;
		}

		public class C1P : IDisposable
		{
			public int X { get; set; }

			[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
			public int Y { get; set; }

			[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
			public object O; // make it not safe

			public void Dispose()
			{
			}
		}

		public class C2P : C1P
		{
			[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
			public new int X { get; set; }

			[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
			public int Z { get; set; }
		}

		public struct S1 : IDisposable
		{
			public C1 X { get; set; }

			public int F;

			public void Dispose()
			{
			}
		}

		public struct S2 : IDisposable
		{
			public IDisposable X { get; set; }

			public void Dispose()
			{
			}
		}

		public class C3
		{
			public C1 X { get; set; }
		}

		[TestMethod]
		public void Descendant_Should_Be_Cloned()
		{
			var c2 = new C2();
			c2.X = 1;
			c2.Y = 2;
			c2.Z = 3;
			var c1 = c2 as C1;
			c1.X = 4;
			var cloned = c1.DeepClone();
			Assert.AreEqual(cloned.GetType(), typeof(C2));
			Assert.AreEqual(cloned.X, (4));
			Assert.AreEqual(cloned.Y, (2));
			Assert.AreEqual(((C2)cloned).Z, (3));
			Assert.AreEqual(((C2)cloned).X, (1));
		}

		[TestMethod]
		public void Class_Should_Be_Cloned_With_Parents()
		{
			var c2 = new C2P();
			c2.X = 1;
			c2.Y = 2;
			c2.Z = 3;
			var c1 = c2 as C1P;
			c1.X = 4;
			var cloned = c2.DeepClone();
			c2.X = 100;
			c2.Y = 100;
			c2.Z = 100;
			c1.X = 100;
			Assert.AreEqual(cloned.GetType(), typeof(C2P));
			Assert.AreEqual(((C1P)cloned).X, (4));
			Assert.AreEqual(cloned.Y, (2));
			Assert.AreEqual(cloned.Z, (3));
			Assert.AreEqual(cloned.X, (1));
		}

		public struct S3
		{
			public C1P X { get; set; }

			public C1P Y { get; set; }
		}

		[TestMethod]
		public void Struct_Should_Be_Cloned_With_Class_With_Parents()
		{
			var c2 = new S3();
			c2.X = new C1P();
			c2.Y = new C2P();

			c2.X.X = 1;
			c2.X.Y = 2;
			c2.Y.X = 3;
			c2.Y.Y = 4;
			((C2P)c2.Y).X = 5;
			((C2P)c2.Y).Z = 6;
			var cloned = c2.DeepClone();
			c2.X.X = 100;
			c2.X.Y = 200;
			c2.Y.X = 300;
			c2.Y.Y = 400;
			((C2P)c2.Y).X = 500;
			((C2P)c2.Y).Z = 600;
			Assert.AreEqual(cloned.GetType(), typeof(S3));
			Assert.AreEqual(cloned.X.X, (1));
			Assert.AreEqual(cloned.X.Y, (2));
			Assert.AreEqual(cloned.Y.X, (3));
			Assert.AreEqual(cloned.Y.Y, (4));
			Assert.AreEqual(((C2P)cloned.Y).X, (5));
			Assert.AreEqual(((C2P)cloned.Y).Z, (6));
		}

		[TestMethod]
		public void Descendant_In_Array_Should_Be_Cloned()
		{
			var c1 = new C1();
			var c2 = new C2();
			var arr = new[] { c1, c2 };

			var cloned = arr.DeepClone();
			Assert.AreEqual(cloned[0].GetType(), typeof(C1));
			Assert.AreEqual(cloned[1].GetType(), typeof(C2));
		}

		[TestMethod]
		public void Struct_Casted_To_Interface_Should_Be_Cloned()
		{
			var s1 = new S1();
			s1.F = 1;
			var disp = s1 as IDisposable;
			var cloned = disp.DeepClone();
			s1.F = 2;
			Assert.AreEqual(cloned.GetType(), typeof(S1));
			Assert.AreEqual(((S1)cloned).F, (1));
		}

		public IDisposable CCC(IDisposable xx)
		{
			var x = (S1)xx;
			return x;
		}

		[TestMethod]
		public void Class_Casted_To_Object_Should_Be_Cloned()
		{
			var c3 = new C3();
			c3.X = new C1();
			var obj = c3 as object;
			var cloned = obj.DeepClone();
			Assert.AreEqual(cloned.GetType(), typeof(C3));
			Assert.AreNotEqual(c3, (cloned));
			Assert.AreNotEqual(((C3)cloned).X, null);
			Assert.AreNotEqual(((C3)cloned).X, (c3.X));
		}

		[TestMethod]
		public void Class_Casted_To_Interface_Should_Be_Cloned()
		{
			var c1 = new C1();
			var disp = c1 as IDisposable;
			var cloned = disp.DeepClone();
			Assert.AreNotEqual(c1, (cloned));
			Assert.AreEqual(cloned.GetType(), typeof(C1));
		}

		[TestMethod]
		public void Struct_Casted_To_Interface_With_Class_As_Interface_Should_Be_Cloned()
		{
			var s2 = new S2();
			s2.X = new C1();
			var disp = s2 as IDisposable;
			var cloned = disp.DeepClone();
			Assert.AreEqual(cloned.GetType(), typeof(S2));
			Assert.AreEqual(((S2)cloned).X.GetType(), typeof(C1));
			Assert.AreNotEqual(((S2)cloned).X, (s2.X));
		}

		[TestMethod]
		public void Array_Of_Struct_Casted_To_Interface_Should_Be_Cloned()
		{
			var s1 = new S1();
			var arr = new IDisposable[] { s1, s1 };
			var clonedArr = arr.DeepClone();
			Assert.AreEqual(clonedArr[0], (clonedArr[1]));
		}

		public class Safe1
		{
		}

		public class Safe2
		{
		}

		public class Unsafe1 : Safe1
		{
			[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
			public object X;
		}

		public class V1
		{
			[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
			public Safe1 Safe;
		}

		public class V2
		{
			[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
			public Safe1 Safe;

 			public V2(string x)
			{
			}
		}

		// these tests are overlapped by others, but for future can be helpful
		[TestMethod]
		public void Class_With_Safe_Class_Should_Be_Cloned()
		{
			var v = new V1();
			v.Safe = new Safe1();
			var v2 = v.DeepClone();
			Assert.AreEqual(v.Safe == v2.Safe, false);
		}

		[TestMethod]
		public void Class_With_Safe_Class_Should_Be_Cloned_No_Default_Constructor()
		{
			var v = new V2("X");
			v.Safe = new Safe1();
			var v2 = v.DeepClone();
			Assert.AreEqual(v.Safe == v2.Safe, false);
		}

		[TestMethod]
		public void Class_With_UnSafe_Class_Should_Be_Cloned()
		{
			var v = new V1();
			v.Safe = new Unsafe1();
			var v2 = v.DeepClone();
			Assert.AreEqual(v.Safe == v2.Safe, false);
			Assert.AreEqual(v2.Safe.GetType(), (typeof(Unsafe1)));
		}

		[TestMethod]
		public void Class_With_UnSafe_Class_Should_Be_Cloned_No_Default_Constructor()
		{
			var v = new V2("X");
			v.Safe = new Unsafe1();
			var v2 = v.DeepClone();
			Assert.AreEqual(v.Safe == v2.Safe, false);
			Assert.AreEqual(v2.Safe.GetType(), (typeof(Unsafe1)));
		}
	}
}
