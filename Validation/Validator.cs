namespace Validation;

public class Validator<T> : IValidator<T>
{
    private readonly IEnumerable<IValidator<T>> _validators;

    public Validator(IEnumerable<IValidator<T>> validators)
    {
        _validators = validators;
    }
        
    public IEnumerable<ValidationResult> Validate(T t)
    {
        return _validators.SelectMany(validator => validator.Validate(t));
    }
}