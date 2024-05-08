using System.Linq.Expressions;

namespace Captivlink.Infrastructure.Utility
{
    public class ExpressionHelpers
    {
        public static Expression<Func<T, bool>> CreateEqualExpression<T>(IDictionary<string, object> filters)
        {
            var param = Expression.Parameter(typeof(T), "p");
            Expression? body = null;
            foreach (var pair in filters)
            {
                var member = Expression.Property(param, pair.Key);
                var constant = Expression.Constant(pair.Value);
                var expression = Expression.Equal(member, constant);
                body = body == null ? expression : Expression.AndAlso(body, expression);
            }
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}
