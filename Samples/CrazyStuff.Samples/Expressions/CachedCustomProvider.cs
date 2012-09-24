using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CrazyStuff.Expressions;

namespace CrazyStuff.Samples.Expressions
{
    /// <summary>
    /// Decorator that adds cache to <see cref="ICustomProvider"/> interface.
    /// </summary>
    public sealed class CachedCustomProvider : ICustomProvider
    {
        private readonly ICustomProvider _customProvider;

        public CachedCustomProvider(ICustomProvider customProvider)
        {
            Contract.Requires(customProvider != null);

            _customProvider = customProvider;
        }

        public Task<Order> GetOrder(int orderId, string customerName)
        {
            return GetFromCacheOrUpdate(() => _customProvider.GetOrder(orderId, customerName));
        }

        private Task<T> GetFromCacheOrUpdate<T>(Expression<Func<Task<T>>> expression)
        {
            // Getting operation id by the expression
            var methodInfo = ExpressionParser.ProcessMethodCallExpression(expression);
            lock (_cacheSyncRoot)
            {
                Console.WriteLine("Looking result into cache. Cache size is {0}", _cache.Count);
                // Looking for the cached value first
                Task cachedTask;
                if (_cache.TryGetValue(methodInfo, out cachedTask))
                {
                    Console.WriteLine("Item not found");
                    return (Task<T>) cachedTask;
                }

                // Compile and execute expression to obtain real task
                var task = expression.Compile()();
                _cache[methodInfo] = task;

                // Clearing cache when task finishes
                task.ContinueWith(t => _cache.Remove(methodInfo));
                return task;
            }
        }

        public Task<int> GetNextId()
        {
            return GetFromCacheOrUpdate(() => _customProvider.GetNextId());
        }

        public Task<string> GetCustomerName(int id)
        {
            return GetFromCacheOrUpdate(() => _customProvider.GetCustomerName(id));
        }
    


    private readonly Dictionary<MethodCallInfo, Task> _cache =
            new Dictionary<MethodCallInfo, Task>();
        private readonly object _cacheSyncRoot = new object();

    }
}