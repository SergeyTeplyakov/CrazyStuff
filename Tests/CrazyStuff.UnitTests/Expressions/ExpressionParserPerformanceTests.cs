using System;
using System.Diagnostics;
using System.Linq.Expressions;
using CrazyStuff.Expressions;
using NUnit.Framework;

namespace CrazyStuff.UnitTests.Expressions
{
    [TestFixture]
    public class ExpressionParserPerformanceTests
    {
        const int Iterations = 10000;

        private class Foo
        {
            public int SomeCustomMethod(int i, string s, double d)
            {
                throw new NotImplementedException();
            }

            public int GetValue() { return 42; }
            public string S { get { return "1"; } }
        }

        public int SomeCustomMethod(int i, string s, double d)
        {
            throw new NotImplementedException();
        }

        [Ignore("Performance tests should be ran manually")]
        [Test]
        public void TestPerformance()
        {
            var simpleExpressions = (Expression<Func<int>>) (() => 
                    SomeCustomMethod(42, "Some string", 2.0));

            var foo = new Foo();

            var simpleClosure = (Expression<Func<int>>)(() =>
                    foo.SomeCustomMethod(42, "12345", 2.0));

            var complexClosure = (Expression<Func<int>>) (() => 
                    foo.SomeCustomMethod(foo.GetValue(), foo.S, 2.0));

            MeasureParseExpressions("Simple expression", simpleExpressions);
            MeasureParseExpressions("Simple closure expression", simpleClosure);
            MeasureParseExpressions("Complex closure expression", complexClosure);

        }

        private static void MeasureParseExpressions<T>(string name, Expression<Func<T>> expression)
        {
            var mi = ExpressionParser.ProcessMethodCallExpression(expression);
            Console.WriteLine("Measuring '{0}'", name);

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < Iterations; i++)
            {
                ExpressionParser.ProcessMethodCallExpression(expression);
            }
            stopwatch.Stop();

            Console.WriteLine("'{0}' parses takes {1}ms", name, stopwatch.ElapsedMilliseconds);
        }

    }
}