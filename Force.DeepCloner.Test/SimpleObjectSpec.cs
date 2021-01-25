using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using Force.DeepCloner.Tests.Objects;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Force.DeepCloner.Tests
{
	[TestClass]
	public class SimpleObjectSpec : BaseTest
	{
		public SimpleObjectSpec() { }
		public SimpleObjectSpec(bool isSafeInit)
			: base(isSafeInit)
		{
		}

		[TestMethod]
		public void SimpleObject_Should_Be_Cloned()
		{
			var obj = new TestObject1 { Int = 42, Byte = 42, Short = 42, Long = 42, DateTime = new DateTime(2001, 01, 01), Char = 'X', Decimal = 1.2m, Double = 1.3, Float = 1.4f, String = "test1", UInt = 42, ULong = 42, UShort = 42, Bool = true, IntPtr = new IntPtr(42), UIntPtr = new UIntPtr(42), Enum = AttributeTargets.Delegate };

			var cloned = obj.DeepClone();
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

		public struct S1
		{
			public int A;
		}

		public struct S2
		{
			public S3 S;
		}

		public struct S3
		{
			public bool B;
		}

		[TestMethod]
		public void SimpleStruct_Should_Be_Cloned()
		{
			var s1 = new S1 { A = 1 };
			var cloned = s1.DeepClone();
			Assert.AreEqual(cloned.A, (1));
		}

		[TestMethod]
		public void Simple_Struct_With_Child_Should_Be_Cloned()
		{
			var s1 = new S2 { S = new S3 { B = true } };
			var cloned = s1.DeepClone();
			Assert.AreEqual(cloned.S.B, (true));
		}

		public class ClassWithNullable
		{
			public int? A { get; set; }

			public long? B { get; set; }
		}

		[TestMethod]
		public void Nullable_Shoild_Be_Cloned()
		{
			var c = new ClassWithNullable { B = 42 };
			var cloned = c.DeepClone();
			Assert.AreEqual(cloned.A, null);
			Assert.AreEqual(cloned.B, (42));
		}

		public class C1
		{
			public C2 C { get; set; }
		}

		public class C2
		{
		}

		public class C3
		{
			public string X { get; set; }
		}

		[TestMethod]
		public void Class_Should_Be_Cloned()
		{
			var c1 = new C1();
			c1.C = new C2();
			var cloned = c1.DeepClone();
			Assert.AreNotEqual(cloned.C, null);
			Assert.AreNotEqual(cloned.C, (c1.C));
		}

		public struct S4
		{
			public C2 C;

			public int F;
		}

		[TestMethod]
		public void StructWithClass_Should_Be_Cloned()
		{
			var c1 = new S4();
			c1.F = 1;
			c1.C = new C2();
			var cloned = c1.DeepClone();
			c1.F = 2;
			Assert.AreNotEqual(cloned.C, null);
			Assert.AreEqual(cloned.F, (1));
		}

		[TestMethod]
		public void Privitive_Should_Be_Cloned()
		{
			Assert.AreEqual(3.DeepClone(), (3));
			Assert.AreEqual('x'.DeepClone(), ('x'));
			Assert.AreEqual("xxxxxxxxxx yyyyyyyyyyyyyy".DeepClone(), ("xxxxxxxxxx yyyyyyyyyyyyyy"));
			Assert.AreEqual(string.Empty.DeepClone(), (string.Empty));
			Assert.AreEqual(ReferenceEquals("y".DeepClone(), "y"), true);
			Assert.AreEqual(DateTime.MinValue.DeepClone(), (DateTime.MinValue));
			Assert.AreEqual(AttributeTargets.Delegate.DeepClone(), (AttributeTargets.Delegate));
			Assert.AreEqual(((object)null).DeepClone(), null);
			var obj = new object();
			Assert.AreNotEqual(obj.DeepClone(), null);
			Assert.AreEqual(true.DeepClone(), true);
			Assert.AreEqual(((object)true).DeepClone(), true);
			Assert.AreEqual(obj.DeepClone().GetType(), (typeof(object)));
			Assert.AreNotEqual(obj.DeepClone(), (obj));
		}

		[TestMethod]
		public void Guid_Should_Be_Cloned()
		{
			var g = Guid.NewGuid();
			Assert.AreEqual(g.DeepClone(), (g));
		}
		

		[TestMethod]
		public void String_In_Class_Should_Not_Be_Cloned()
		{
			var c = new C3 { X = "aaa" };
			var cloned = c.DeepClone();
			Assert.AreEqual(cloned.X, (c.X));
			Assert.AreEqual(ReferenceEquals(cloned.X, c.X), true);
		}

		public sealed class C6
		{
			public readonly int X = 1;

			private readonly object y = new object();

			// it is struct - and it can't be null, but it's readonly and should be copied
			// also it private to ensure it copied correctly
#pragma warning disable 169
			private readonly StructWithObject z;
#pragma warning restore 169

			public object GetY()
			{
				return y;
			}
		}

		public struct StructWithObject
		{
			public readonly object Z;
		}

		[TestMethod]
		public void Object_With_Readonly_Fields_Should_Be_Cloned()
		{
			var c = new C6();
			var clone = c.DeepClone();
			Assert.AreNotEqual(clone, (c));
			Assert.AreEqual(clone.X, (1));
			Assert.AreNotEqual(clone.GetY(), null);
			Assert.AreNotEqual(clone.GetY(), (c.GetY()));
			Assert.AreNotEqual(clone.GetY(), (c.GetY()));
		}

		public class VirtualClass1
		{
			public virtual int A { get; set; }

			public virtual int B { get; set; }

			// not safe
			public object X { get; set; }
		}

		public class VirtualClass2 : VirtualClass1
		{
			public override int B { get; set; }
		}

		[TestMethod]
		public void Class_With_Virtual_Methods_Should_Be_Cloned()
		{
			var v2 = new VirtualClass2();
			v2.A = 1;
			v2.B = 2;
			var v1 = v2 as VirtualClass1;
			v1.A = 3;
			var clone = v1.DeepClone() as VirtualClass2;
			v2.B = 0;
			v2.A = 0;
			Assert.AreEqual(clone.B, (2));
			Assert.AreEqual(clone.A, (3));
		}

		[TestMethod]
		public void DbNull_Should_Not_Be_Cloned()
		{
			var v = DBNull.Value;
			Assert.AreEqual(v == v.DeepClone(), true);
			Assert.AreEqual(v == v.ShallowClone(), true);
		}
		
		public class EmptyClass {}
		
		[TestMethod]
		public void Empty_Should_Not_Be_Cloned()
		{
			var v = new EmptyClass();
			Assert.AreEqual(ReferenceEquals(v, v.DeepClone()), false);
			Assert.AreEqual(ReferenceEquals(v, v.ShallowClone()), false);
		}
		
		[TestMethod]
		public void MethodInfo_Should_Not_Be_Cloned()
		{
#if NETCORE13
			var v = GetType().GetTypeInfo().GetMethod("MethodInfo_Should_Not_Be_Cloned");
#else
			var v = GetType().GetMethod("MethodInfo_Should_Not_Be_Cloned");
#endif
			Assert.AreEqual(ReferenceEquals(v, v.DeepClone()), true);
			Assert.AreEqual(ReferenceEquals(v, v.ShallowClone()), true);
		}

		public class Readonly1
		{
			public readonly object X;
			
			public object Z = new object();

			public Readonly1(string x)
			{
				X = x;
			}
		}

		[TestMethod]
		public void Readonly_Field_Should_Remain_ReadOnly()
		{
			var c = new Readonly1("Z").DeepClone();
			Assert.AreEqual(c.X, ("Z"));
			Assert.AreEqual(typeof(Readonly1).GetField("X").IsInitOnly, true);
		}
	}
}
