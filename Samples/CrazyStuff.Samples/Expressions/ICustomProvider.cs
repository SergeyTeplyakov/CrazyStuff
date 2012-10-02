using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CrazyStuff.Expressions;
using NUnit.Framework;

namespace CrazyStuff.Samples.Expressions
{
    public interface ICustomProvider
    {
        Task<int> GetNextId();
        Task<string> GetCustomerName(int id);
        Task<Order> GetOrder(int orderId, string customerName);
    }
}