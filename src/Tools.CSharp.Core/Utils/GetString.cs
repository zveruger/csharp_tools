using System;
using System.Linq.Expressions;

namespace Tools.CSharp.Utils
{
    public static class GetString
    {
        #region private
        private static string _GetMemberName(Expression expression, bool fullName)
        {
            if (expression == null)
            { return string.Empty; }

            switch(expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    {
                        var memberExpression = (MemberExpression)expression;
                        var supername = fullName ? _GetMemberName(memberExpression.Expression, true) : string.Empty;
                        return string.IsNullOrEmpty(supername) ? memberExpression.Member.Name : string.Concat(supername, '.', memberExpression.Member.Name);
                    }
                case ExpressionType.Call:
                    {
                        var callExpression = (MethodCallExpression)expression;
                        return callExpression.Method.Name;
                    }
                case ExpressionType.Convert:
                    {
                        var unaryExpression = (UnaryExpression)expression;
                        return _GetMemberName(unaryExpression.Operand, fullName);
                    }
                case ExpressionType.Constant:
                case ExpressionType.Parameter:
                    { return string.Empty; }
                default:
                    throw new ArgumentException("The expression is not a member access or method call expression");
            }
        }
        #endregion
        //---------------------------------------------------------------------
        public static string Of<T>()
        {
            return typeof(T).Name;
        }
        public static string Of(LambdaExpression expression, bool fullName = true)
        {
            if (expression == null)
            { throw new ArgumentNullException(nameof(expression)); }

            return _GetMemberName(expression.Body, fullName);
        }
        //---------------------------------------------------------------------
    }
}