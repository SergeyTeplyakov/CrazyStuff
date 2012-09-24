using System.Collections.Generic;
using CrazyStuff.Expressions;
using NUnit.Framework;
using System.Linq;

namespace CrazyStuff.Samples.Expressions
{
    public class CustomTestCaseData : TestCaseData
    {
        public CustomTestCaseData(MethodCallInfo callInfo)
            : base(callInfo.MethodName, callInfo.Arguments)
        { }

        public string MethodName { get { return (string)Arguments[0]; } }
    }


    [TestFixture]
    public class CustomProviderObjectMother
    {
        public static IEnumerable<CustomTestCaseData> GetCustomProviderTestData()
        {
            // This is similar to the following:
            // yield return new TestCaseData("GetOrder", new object[] {42, "John Doe"});
            yield return new CustomTestCaseData(
                ExpressionParser.ProcessMethodCallExpression(
                    (ICustomProvider cp) => cp.GetOrder(42, "John Doe")));

            yield return new CustomTestCaseData(
                ExpressionParser.ProcessMethodCallExpression(
                    (ICustomProvider cp) => cp.GetCustomerName(42)));

            yield return new CustomTestCaseData(
                ExpressionParser.ProcessMethodCallExpression(
                    (ICustomProvider cp) => cp.GetNextId()));
        }

        [Test]
        public void Test_GetCustomProviderTestData_Returns_Data_For_All_ICustomProvider_Methods()
        {
            // Arrange
            var testData = GetCustomProviderTestData()
                            .Select(pd => pd.MethodName)
                            .OrderBy(m => m)
                            .ToList();

            var customProviderMethods = typeof (ICustomProvider).GetMethods()
                                            .Select(mi => mi.Name)
                                            .OrderBy(m => m)
                                            .ToList();

            // Assert
            CollectionAssert.AreEqual(testData, customProviderMethods);
        }
    }
}