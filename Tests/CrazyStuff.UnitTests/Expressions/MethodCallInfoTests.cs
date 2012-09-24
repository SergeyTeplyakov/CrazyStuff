using System.Reflection.Emit;
using CrazyStuff.Expressions;
using NUnit.Framework;

namespace CrazyStuff.UnitTests.Expressions
{
    /// <summary>
    /// Set of unit tests for the <see cref="MethodCallInfo"/> structure.
    /// </summary>
    [TestFixture]
    public class MethodCallInfoTests
    {
        [Test]
        public void Test_Two_Objects_With_The_Same_Arguments_Are_Equal()
        {
            // Arrange
            var callInfo1 = new MethodCallInfo("Foo", new object[] {1, "foo", 2.0});
            var callInfo2 = new MethodCallInfo("Foo", new object[] {1, "foo", 2.0});

            // Assert
            Assert.AreEqual(callInfo1, callInfo2);
        }
        
        [Test]
        public void Test_Two_Objects_With_The_Same_Arguments_Returns_The_Same_HashCode()
        {
            // Arrange
            var callInfo1 = new MethodCallInfo("Foo", new object[] {1, "foo", 2.0});
            var callInfo2 = new MethodCallInfo("Foo", new object[] {1, "foo", 2.0});

            // Assert
            Assert.AreEqual(callInfo1.GetHashCode(), callInfo2.GetHashCode());
        }
    }
}