using System.Linq.Expressions;

namespace Validation;

public static class PropertyHelper
{
    public static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }

        throw new ArgumentException("Expression is not a valid member expression");
    }    
}