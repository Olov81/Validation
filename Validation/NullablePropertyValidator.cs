using System.Linq.Expressions;

namespace Validation;

public class NullablePropertyValidator<T, TProperty> : IValidator<T> where TProperty : struct
{
    private readonly Expression<Func<T, TProperty?>> _getProperty;
    private readonly bool _isRequired;

    public NullablePropertyValidator(Expression<Func<T, TProperty?>> getProperty, bool isRequired)
    {
        _getProperty = getProperty;
        _isRequired = isRequired;
        InnerValidator = new PropertyValidator<T, TProperty>(t => getProperty.Compile()(t).Value);
    }

    public PropertyValidator<T, TProperty> InnerValidator { get; }
        
    public IEnumerable<ValidationResult> Validate(T t)
    {
        var value = _getProperty.Compile()(t);

        if (value == null)
        {
            return _isRequired ? CreateValidationResult(false) : new List<ValidationResult>();
        }

        return CreateValidationResult(true).Concat(InnerValidator.Validate(t));
    }

    private IEnumerable<ValidationResult> CreateValidationResult(bool isValid)
    {
        return new[] { new ValidationResult(PropertyHelper.GetPropertyName(_getProperty), isValid, "Required") };
    }
}