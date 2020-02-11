using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StringEquals
{
    public class StringEqualsVisitor : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == nameof(QueryableExtensions.EqualsExt))
            {
                return BuildStringEqualsExpression(node);
            }

            return base.VisitMethodCall(node);
        }

        /// <summary>
        /// Convert leftOperator == rightOperator to optimize entity framework linq.
        /// Suppose leftOperator is string constant and rightOperator is entity property
        /// <para>If leftOperator is null or empty: rightOperator == null </para>
        /// <para>If leftOperator is not null and not empty: rightOperator != null && leftOperator == rightOperator</para>
        /// </summary>
        /// <param name="node"></param>
        /// <returns>
        /// <para>If leftOperator is null or empty: rightOperator == null </para>
        /// <para>If leftOperator is not null and not empty: rightOperator != null && leftOperator == rightOperator</para>
        /// </returns>
        private Expression BuildStringEqualsExpression(MethodCallExpression node)
        {
            var arg0 = node.Arguments[0] is MemberExpression ? node.Arguments[0] as MemberExpression : null;
            var arg1 = node.Arguments[1] is MemberExpression ? node.Arguments[1] as MemberExpression : null;
            if (arg0 == null && arg1 == null)
            {
                throw new NotSupportedException();
            }
            var propertyNode = arg0 != null && arg0.Expression is ConstantExpression ? node.Arguments[1] : node.Arguments[0];
            var constantNode = arg0 != null && arg0.Expression is ConstantExpression ? arg0 : arg1;
            var fieldExp = constantNode as MemberExpression;
            var objectValue = (fieldExp.Expression as ConstantExpression).Value;
            var compareVal = objectValue.GetType().GetField(fieldExp.Member.Name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(objectValue) + "";
            var nullExp = Expression.Constant(null);
            if (string.IsNullOrEmpty(compareVal))
            {
                return Expression.Equal(propertyNode, nullExp);
            }
            var notNullExp = Expression.NotEqual(propertyNode, nullExp);
            var propEqCstExp = Expression.Equal(propertyNode, constantNode);
            return Expression.AndAlso(notNullExp, propEqCstExp);
        }
    }

}
