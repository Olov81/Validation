using System.Linq.Expressions;

namespace Validation;

public class ValidatorBuilder<T>
{
    private readonly List<IValidator<T>> _validators = new();
    
    public static Validator<T> Create(params Action<ValidatorBuilder<T>>[] configure)
    {
        var builder = new ValidatorBuilder<T>();
            
        foreach (var action in configure)
        {
            action(builder);
        }

        return new Validator<T>(builder._validators);
    }

    public PropertyValidator<T, TProperty> Value<TProperty>(Expression<Func<T, TProperty>> propertySelector) where TProperty : struct
    {
        var propertyValidator = new PropertyValidator<T, TProperty>(
            propertySelector.Compile(),
            GetPropertyName(propertySelector));
        _validators.Add(propertyValidator);
        return propertyValidator;
    }

    public PropertyValidator<T, TProperty> Require<TProperty>(Expression<Func<T, TProperty?>> propertySelector) where TProperty : struct => AddNullable(propertySelector, true);
    public PropertyValidator<T, TProperty> Optional<TProperty>(Expression<Func<T, TProperty?>> propertySelector) where TProperty : struct => AddNullable(propertySelector, false);
        
    public PropertyValidator<T, TProperty> Require<TProperty>(Expression<Func<T, TProperty?>> propertySelector) where TProperty : class => AddNullable(propertySelector, true);
    public PropertyValidator<T, TProperty> Optional<TProperty>(Expression<Func<T, TProperty?>> propertySelector) where TProperty : class => AddNullable(propertySelector, false);

    private PropertyValidator<T, TProperty> AddNullable<TProperty>(Expression<Func<T, TProperty?>> propertySelector, bool isRequired) where TProperty : struct
    {
        var validator = new NullablePropertyValidator<T, TProperty>(
            propertySelector.Compile(),
            GetPropertyName(propertySelector),
            isRequired);
        _validators.Add(validator);
        return validator.InnerValidator;
    }
        
    private PropertyValidator<T, TProperty> AddNullable<TProperty>(Expression<Func<T, TProperty?>> propertySelector, bool isRequired) where TProperty : class
    {
        var validator = new ReferencePropertyValidator<T, TProperty>(
            propertySelector.Compile(),
            GetPropertyName(propertySelector),
            isRequired);
        _validators.Add(validator);
        return validator.InnerValidator;
    }
    
    private static string GetPropertyName<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }

        throw new ArgumentException("Expression is not a valid member expression");
    }   
}