//
// System.Reflection.ConstructorInfo Test Cases
//
// Authors:
//  Gert Driesen (drieseng@users.sourceforge.net)
//
// (c) 2007 Gert Driesen
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Reflection;
using System.Runtime.Serialization;

using NUnit.Framework;

namespace MonoTests.System.Reflection
{
	[TestFixture]
	public class ConstructorInfoTest
	{
		[Test]
		public void Invoke ()
		{
			ConstructorInfo ctor = typeof (Foo).GetConstructor (
				BindingFlags.Public | BindingFlags.Instance, null,
				Type.EmptyTypes, null);
			Foo foo = ctor.Invoke (new object [0]) as Foo;
			Assert.IsNotNull (foo, "#1");
		}

		[Test]
		public void InvokeAbstract ()
		{
			object obj = FormatterServices.GetUninitializedObject (typeof (Foo));
			ConstructorInfo ctor = typeof (AbstractFoo).GetConstructor (
				BindingFlags.Instance | BindingFlags.NonPublic, null, 
				Type.EmptyTypes, null);
			Assert.IsNotNull (ctor, "#A1");
			object ret = ctor.Invoke (obj, new object [0]);
			Assert.IsNull (ret, "#A2");

			try {
				ctor.Invoke (null, new object [0]);
				Assert.Fail ("#B1");
			} catch (TargetException ex) {
				// Non-static method requires a target
				Assert.AreEqual (typeof (TargetException), ex.GetType (), "#B2");
				Assert.IsNull (ex.InnerException, "#B3");
				Assert.IsNotNull (ex.Message, "#B4");
			}

			try {
				ctor.Invoke (new object [0]);
				Assert.Fail ("#C1");
			} catch (MemberAccessException ex) {
				// Cannot create an abstract class
				Assert.AreEqual (typeof (MemberAccessException), ex.GetType (), "#C2");
				Assert.IsNull (ex.InnerException, "#C3");
				Assert.IsNotNull (ex.Message, "#C4");
			}
		}

		abstract class AbstractFoo
		{
		}

		class Foo : AbstractFoo
		{
		}

#if NET_2_0
		[Test]
		[ExpectedException (typeof (MemberAccessException))]
		public void InvokeOpenGenericType () {
			typeof (Gen<>).GetConstructor (Type.EmptyTypes).Invoke (null);
		}

		public class Gen<T> {
			public Gen() {
			}
		}
#endif			
	}
}
