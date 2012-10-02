using System;
using System.Collections.Concurrent;
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
        private readonly Dictionary<MethodCallInfo, Task> _cache = new Dictionary<MethodCallInfo, Task>();
        private readonly object _cacheSyncRoot = new object();

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
            // Получаем идентификатор операции
            var methodInfo = ExpressionParser.ProcessMethodCallExpression(expression);

            Console.WriteLine("MethodInfo: {0}",methodInfo);
            lock(_cacheSyncRoot)
            {
                
                Console.WriteLine(_cache.Count);

                Task cachedTask;
                if (_cache.TryGetValue(methodInfo, out cachedTask))
                {
                    return (Task<T>) cachedTask;
                }

                // Получаем задачу
                var newTask = expression.Compile()();
                _cache[methodInfo] = newTask;

                // Подписываемся на продолжение и удалем ее
                newTask.ContinueWith(t =>
                        {
                            lock (_cacheSyncRoot)
                                _cache.Remove(methodInfo);
                        });

                return newTask;
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
    }
}