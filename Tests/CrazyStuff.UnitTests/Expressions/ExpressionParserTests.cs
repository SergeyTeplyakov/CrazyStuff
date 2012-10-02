using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CrazyStuff.Expressions;
using CrazyStuff.Samples.Expressions;
using NUnit.Framework;

namespace CrazyStuff.UnitTests.Expressions
{
    /// <summary>
    /// Set of unit tests for the <see cref="ExpressionParser"/> class.
    /// </summary>
    [TestFixture]
    public class ExpressionParserTests
    {
        [Test]
        public void Test_ParseMethodCallExpression_Returns_Correct_Result_For_Func_Expression()
        {
            // ExpressionParser parses specified expression and produces JobId
            // by method name and set of parameters.

            // Act
            var methodCallInfo = ExpressionParser.ProcessMethodCallExpression(
                (ICustomProvider cp) => cp.GetOrder(42, "John Doe"));

            // Assert
            Assert.That(methodCallInfo.MethodName, Is.EqualTo("GetOrder"));
            CollectionAssert.AreEqual(new object[] { 42, "John Doe"}, methodCallInfo.Arguments);
        }

        [Test]
        public void Test_ParseMethodCallExpression_Returns_Correct_Result_With_Additional_Method_Calls()
        {
            // Act
            var methodCallInfo = ExpressionParser.ProcessMethodCallExpression(
                (ICustomProvider cp) => cp.GetOrder(int.Parse("42"), "John Doe"));

            // Assert
            Assert.That(methodCallInfo.MethodName, Is.EqualTo("GetOrder"));
            CollectionAssert.AreEqual(new object[] { 42, "John Doe" }, methodCallInfo.Arguments);
        }

        [Test]
        public void Test_ParseMethodCallExpression_Returns_Correct_Result_For_Simple_Expression()
        {
            // ExpressionParser parses specified expression and produces JobId
            // by method name and set of parameters.

            // Act
            var methodCallInfo = ExpressionParser.ProcessMethodCallExpression(
                () => SomeCustomMethod(42, "Some String", 2.0));

            // Assert
            Assert.That(methodCallInfo.MethodName, Is.EqualTo("SomeCustomMethod"));
            CollectionAssert.AreEqual(new object[] { 42, "Some String", 2.0}, methodCallInfo.Arguments);
        }

        [Test]
        public void Test_ParseMethodCallExpression_With_Simple_Closure()
        {
            // ExpressionParse should parse expressions that closes over local variable
            // Arrange
            string someString = "12345";

            // Act
            var methodCallInfo = ExpressionParser.ProcessMethodCallExpression(
                () => int.Parse(someString));

            // Assert
            Assert.That(methodCallInfo.MethodName, Is.EqualTo("Parse"));
            CollectionAssert.AreEqual(new object[] {someString}, methodCallInfo.Arguments);
        }

        private class Foo
        {
            public int GetStringLength(string s) { return s.Length; }
            public string S { get { return "42"; } }
            public string GetString() { return "42"; }
        }

        [Test]
        public void Test_ParseMethodCallExpression_With_Closed_Object()
        {
            // Arrange
            var foo = new Foo();

            // Act
            var methodCallInfo = ExpressionParser.ProcessMethodCallExpression(
                () => foo.GetStringLength(foo.S));

            // Assert
            Assert.That(methodCallInfo.MethodName, Is.EqualTo("GetStringLength"));
            CollectionAssert.AreEqual(new object[] { foo.S }, methodCallInfo.Arguments);
        }
        
        [Test]
        public void Test_ParseMethodCallExpression_With_Complex_Closure()
        {
            // Arrange
            var foo = new Foo();

            // Act
            var methodCallInfo = ExpressionParser.ProcessMethodCallExpression(
                () => foo.GetStringLength(foo.GetString()));

            // Assert
            Assert.That(methodCallInfo.MethodName, Is.EqualTo("GetStringLength"));
            CollectionAssert.AreEqual(new object[] { foo.GetString() }, methodCallInfo.Arguments);
        }

        private Task<int> SomeCustomMethod(int id, string name, double value)
        {
            throw new NotImplementedException();
        }
    }
}