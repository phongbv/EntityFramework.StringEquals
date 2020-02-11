using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StringEquals
{
    public static class QueryableExtensions
    {

        /// <summary>
        /// Convert leftOperator == rightOperator to optimize entity framework linq.
        /// Suppose <paramref name="leftOperator"/> is string constant and <paramref name="rightOperator"/> is entity property
        /// <para></para>
        /// <para>If leftOperator is null or empty: rightOperator == null </para>
        /// <para>If leftOperator is not null and not empty: rightOperator != null && leftOperator == rightOperator</para>
        /// <para> Call <code>InterceptStringEquals()</code> in last Linq </para>
        /// </summary>
        /// <param name="leftOperator"></param>
        /// <param name="rightOperator"></param>
        /// <returns></returns>
        public static bool EqualsExt(this string leftOperator, string rightOperator)
        {
            throw new NotImplementedException();
        }
        public static IQueryable<T> InterceptWith<T>(this IQueryable<T> source, params ExpressionVisitor[] visitors)
        {
            var expression = source.Expression;
            foreach (var item in visitors)
            {
                expression = item.Visit(expression);
            }
            if (expression == source.Expression) return source;
            return source.Provider.CreateQuery<T>(expression);
        }
        public static IQueryable<T> InterceptStringEquals<T>(this IQueryable<T> source)
        {
            var visitor = new StringEqualsVisitor();
            return source.InterceptWith(visitor);
        }
    }

}
