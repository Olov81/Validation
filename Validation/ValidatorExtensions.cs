namespace Validation;

public static class ValidatorExtensions
{
    public static IEnumerable<ValidationResult> Validate<T>(this T t, params Action<ValidatorBuilder<T>>[] configure)
    {
        var validator = ValidatorBuilder<T>.Create(configure);
        return validator.Validate(t);
    }
}