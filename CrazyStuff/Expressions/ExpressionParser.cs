using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;

namespace CrazyStuff.Expressions
{
    /// <summary>
    /// Helper class that parses expression trees into more usable objects.
    /// </summary>
    public static class ExpressionParser
    {
        /// <summary>
        /// Parses specified <paramref name="expression"/> and produces 
        /// easy-to-use <see cref="MethodCallInfo"/> object.
        /// </summary>
        /// <example>
        /// var methodInfo = ProcessMethodCallExpression((ICustomProvider cp) => cp.GetOrder(
        ///     GetCustomerId(), "Customer Name");
        /// // methodInfo.Operation == "GetOrder"
        /// // methodInfo.Arguments[0] == 42 (suppose GetCustomerId() will return 42)
        /// // methodInfo.Arguments[1] == "Customer Name"
        /// More samples could be found for the CrazyStuff.Samples.ICustomProvider interface.
        /// </example>
        public static MethodCallInfo ProcessMethodCallExpression<TInput, TResult>(
            Expression<Func<TInput, TResult>> expression)
        {
            var methodCall = expression.Body as MethodCallExpression;
            Contract.Assert(methodCall != null, 
                "ProcessMethodCallExpression supports only method call expressions");

            // Converting all arguments to Func<object> expressions and evaluate them
            var arguments = from arg in methodCall.Arguments
                            let argAsObj = Expression.Convert(arg, typeof(object))
                            select Expression.Lambda<Func<object>>(argAsObj, null)
                                .Compile()();

            var parameters = arguments.ToArray();

            return new MethodCallInfo(methodCall.Method.Name, parameters);
        }

        /// <summary>
        /// Generates <see cref="MethodCallInfo"/> by input expression.
        /// </summary>
        /// <example>
        /// var methodInfo = ProcessMethodCallExpression(() => SomeOtherMethod(42, "CustomString", CustomEnum.SomeValue));
        /// // methodInfo.Operation == "SomeOtherMethod"
        /// // methodInfo.Arguments[0] == 42
        /// // methodInfo.Arguments[1] == "CustomString"
        /// // methodInfo.Parametes[2] == CustomEnum.SomeValue
        /// </example>
        public static MethodCallInfo ProcessMethodCallExpression<TResult>(Expression<Func<TResult>> expression)
        {
            var methodCall = expression.Body as MethodCallExpression;
            Contract.Assert(methodCall != null, 
                "ProcessMethodCallExpression supports only method call expressions");

            // Converting all arguments to Func<object> expressions and evaluate them
            var arguments = from arg in methodCall.Arguments
                            let argAsObj = Expression.Convert(arg, typeof(object))
                            select Expression.Lambda<Func<object>>(argAsObj, null)
                                .Compile()();

            var parameters = arguments.ToArray();

            return new MethodCallInfo(methodCall.Method.Name, parameters);
        }
    }
}