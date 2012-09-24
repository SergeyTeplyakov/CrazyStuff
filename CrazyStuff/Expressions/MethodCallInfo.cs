using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Text;

namespace CrazyStuff.Expressions
{
    /// <summary>
    /// Represents information of the method like method info and actual parameters.
    /// </summary>
    public struct MethodCallInfo
    {
        public readonly string MethodName;
        public readonly object[] Arguments;

        public MethodCallInfo(string methodName, object[] args)
        {
            if (methodName == null)
                throw new ArgumentNullException("methodName");
            if (args == null)
                throw new ArgumentNullException("args");
            Contract.EndContractBlock();

            MethodName = methodName;
            Arguments = args;
        }
        
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(MethodName != null);
            Contract.Invariant(Arguments != null);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Method name: {0}", MethodName).AppendLine()
                .AppendLine(string.Join(", ", Arguments));
            return sb.ToString();
        }

        public bool Equals(MethodCallInfo other)
        {
            // Use StructuralComparisons for getting "value semantic"
            var arrayComparer = StructuralComparisons.StructuralEqualityComparer;
            return Equals(MethodName, other.MethodName) 
                && arrayComparer.Equals(Arguments, other.Arguments);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MethodCallInfo && Equals((MethodCallInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var arrayComparer = StructuralComparisons.StructuralEqualityComparer;
                return MethodName.GetHashCode()*397 ^ arrayComparer.GetHashCode(Arguments);
            }
        }
    }
}