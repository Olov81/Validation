namespace Validation;

public class NullablePropertyValidator<T, TProperty> : IValidator<T> where TProperty : struct
{
    private readonly Func<T, TProperty?> _getProperty;
    private readonly string _propertyName;
    private readonly bool _isRequired;

    public NullablePropertyValidator(Func<T, TProperty?> getProperty, string propertyName, bool isRequired)
    {
        _getProperty = getProperty;
        _propertyName = propertyName;
        _isRequired = isRequired;
        InnerValidator = new PropertyValidator<T, TProperty>(t => getProperty(t)!.Value, propertyName);
    }

    public PropertyValidator<T, TProperty> InnerValidator { get; }
        
    public IEnumerable<ValidationResult> Validate(T t)
    {
        var value = _getProperty(t);

        if (_isRequired)
        {
            return value == null 
                ? CreateValidationResult(false) 
                : CreateValidationResult(true).Concat(InnerValidator.Validate(t));
        }
        
        return value == null
            ? new List<ValidationResult>() 
            : InnerValidator.Validate(t);
    }

    private IEnumerable<ValidationResult> CreateValidationResult(bool isValid)
    {
        return new[] { new ValidationResult(_propertyName, isValid, "Required") };
    }
}