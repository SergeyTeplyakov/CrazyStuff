using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

            // Converting or evaluating all arguments
            var arguments = from arg in methodCall.Arguments
                            select ProcessExpression(arg);

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

            // Converting or evaluating all arguments
            var arguments = from arg in methodCall.Arguments
                            select ProcessExpression(arg);

            var parameters = arguments.ToArray();

            return new MethodCallInfo(methodCall.Method.Name, parameters);
        }

        private static object ProcessExpression(Expression expression)
        {
            var visitor = new CustomVisitor();
            visitor.Visit(expression);

            if (visitor.Processed)
                return visitor.Value;

            // Can't process this expression with our custom visitor.
            // Using heavy-weight compilation

            var argAsObj = Expression.Convert(expression, typeof (object));

            return Expression.Lambda<Func<object>>(argAsObj, null).Compile()();
        }

        private class CustomVisitor : ExpressionVisitor
        {
            private object _value;

            public bool Processed { get; private set; }
            public object Value
            {
                get { return _value; }
                private set
                {
                    Processed = true;
                    _value = value;
                }
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                Value = node.Value;
                return node;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                // Member expression could be in form of simple fields or
                // properties

                if (node.Expression is ConstantExpression)
                {
                    var ci = (ConstantExpression) node.Expression;
                    var mi = (FieldInfo) node.Member;
                    Value = mi.GetValue(ci.Value);
                }
                else if (node.Member is PropertyInfo)
                {
                    var pi = (PropertyInfo) node.Member;
                    
                    // Visiting subexpression recursivly to obtain 
                    // property objects value (for example, for expression () => foo.X
                    // pi will contain "X" and value - "foo"
                    var visitor = new CustomVisitor();
                    visitor.Visit(node.Expression);
                    var value = visitor.Value;
                    
                    if (visitor.Processed)
                        Value = pi.GetValue(value);
                }

                return node;
            }

            protected override Expression VisitInvocation(InvocationExpression node)
            {
                Value = Expression.Lambda(node).Compile().DynamicInvoke();
                return node;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                var methodInfo = node.Method;
                var visitor = new CustomVisitor();
                visitor.Visit(node.Object);
                if (visitor.Processed)
                {
                    Value = methodInfo.Invoke(visitor.Value, null);
                }
                else
                {
                    Value = Expression.Lambda(node).Compile().DynamicInvoke();
                }
                return node;
            }
        }
    }
}