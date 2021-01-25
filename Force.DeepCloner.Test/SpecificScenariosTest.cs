using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Force.DeepCloner.Tests
{
	[TestClass]
	public class SpecificScenariosTest : BaseTest
	{
		public SpecificScenariosTest() { }
		public SpecificScenariosTest(bool isSafeInit)
			: base(isSafeInit)
		{
		}

		[TestMethod]
		public void Test_ExpressionTree_OrderBy1()
		{
			var q = Enumerable.Range(1, 5).Reverse().AsQueryable().OrderBy(x => x);
			var q2 = q.DeepClone();
			Assert.AreEqual(q2.ToArray()[0], (1));
			Assert.AreEqual(q.ToArray().Length, (5));
		}

		[TestMethod]
		public void Test_ExpressionTree_OrderBy2()
		{
			var l = new List<int> { 2, 1, 3, 4, 5 }.Select(y => new Tuple<int, string>(y, y.ToString(CultureInfo.InvariantCulture)));
			var q = l.AsQueryable().OrderBy(x => x.Item1);
			var q2 = q.DeepClone();
			Assert.AreEqual(q2.ToArray()[0].Item1, (1));
			Assert.AreEqual(q.ToArray().Length, (5));
		}

		

		

		[TestMethod]
		public void Clone_ComObject1()
		{
#if !NETCORE
// ReSharper disable SuspiciousTypeConversion.Global
			var manager = (KnownFolders.IKnownFolderManager)new KnownFolders.KnownFolderManager();
// ReSharper restore SuspiciousTypeConversion.Global
			Guid knownFolderId1;
			Guid knownFolderId2;
			manager.FolderIdFromCsidl(0, out knownFolderId1);
			manager.DeepClone().FolderIdFromCsidl(0, out knownFolderId2);
			Assert.AreEqual(knownFolderId1, (knownFolderId2));
#endif
		}

		[TestMethod]
		public void Clone_ComObject2()
		{
#if !NETCORE
			Type t = Type.GetTypeFromProgID("SAPI.SpVoice");
			var obj = Activator.CreateInstance(t);
			obj.DeepClone();
#endif
		}

		[TestMethod]
		public void Lazy_Clone()
		{
			var lazy = new LazyClass();
			var clone = lazy.DeepClone();
			var v = LazyClass.Counter;
			Assert.AreEqual(clone.GetValue(), ((v + 1).ToString(CultureInfo.InvariantCulture)));
			Assert.AreEqual(lazy.GetValue(), ((v + 2).ToString(CultureInfo.InvariantCulture)));
		}

		public class LazyClass
		{
			public static int Counter;
			
			private readonly LazyRef<object> _lazyValue = new LazyRef<object>(() => (object)(++Counter).ToString(CultureInfo.InvariantCulture));

			public string GetValue()
			{
				return _lazyValue.Value.ToString();
			}
		}


		[TestMethod]
		public void GenericComparer_Clone()
		{
			var comparer = new TestComparer();
			comparer.DeepClone();
		}

		[TestMethod]
		public void Closure_Clone()
		{
			int a = 0;
			Func<int> f = () => ++a;
			var fCopy = f.DeepClone();
			Assert.AreEqual(f(), (1));
			Assert.AreEqual(fCopy(), (1));
			Assert.AreEqual(a, (1));
		}



		private class TestComparer : Comparer<int>
		{
			// make object unsafe to work
			private object _fieldX = new object();

			public override int Compare(int x, int y)
			{
				return x.CompareTo(y);
			}
		}

#if !NETCORE
		public class KnownFolders
		{
			[Guid("8BE2D872-86AA-4d47-B776-32CCA40C7018"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			internal interface IKnownFolderManager
			{
				void FolderIdFromCsidl(int csidl, [Out] out Guid knownFolderID);

				void FolderIdToCsidl([In] [MarshalAs(UnmanagedType.LPStruct)] Guid id, [Out] out int csidl);

				void GetFolderIds();
			}

			[ComImport, Guid("4df0c730-df9d-4ae3-9153-aa6b82e9795a")]
			internal class KnownFolderManager
			{
				// make object unsafe to work
#pragma warning disable 169
				private object _fieldX;
#pragma warning restore 169
			}
		}
#endif
		public sealed class LazyRef<T>
		{
			private Func<T> _initializer;
			private T _value;

			/// <summary>
			///     This API supports the Entity Framework Core infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			public T Value
			{
				get
				{
					if (_initializer != null)
					{
						_value = _initializer();
						_initializer = null;
					}
					return _value;
				}
				set
				{
					_value = value;
					_initializer = null;
				}
			}
			
			public LazyRef(Func<T> initializer)
			{
				_initializer = initializer;
			}
		}
	}
}
