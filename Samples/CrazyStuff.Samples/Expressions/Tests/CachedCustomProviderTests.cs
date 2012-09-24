using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrazyStuff.Expressions;
using NUnit.Framework;

namespace CrazyStuff.Samples.Expressions
{
    [TestFixture]
    public class CachedCustomProviderTests
    {
        [TestCaseSource(typeof(CustomProviderObjectMother), "GetCustomProviderTestData")]
        public void Test_Calling_Method_Twice_Will_Return_The_Same_Result(string method, object[] arguments)
        {
            // Arrange
            ICustomProvider customProvider = new CachedCustomProvider(new CustomProvider());
            var methodInfo = typeof (ICustomProvider).GetMethod(method);
            Assert.IsNotNull(methodInfo, 
                string.Format("ICustomProvider does not contain method '{0}'", method));

            // Act
            // Вызываем наш метод дважды подряд
            var task1 = (Task) methodInfo.Invoke(customProvider, arguments);
            var task2 = (Task) methodInfo.Invoke(customProvider, arguments);

            // Assert
            Assert.IsTrue(ReferenceEquals(task1, task2),
                    "Two subsequent calls to CachedCustomProvider should return the same objects");
        }
    }
}