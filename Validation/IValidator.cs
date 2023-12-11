namespace Validation;

public interface IValidator<in T>
{
    IEnumerable<ValidationResult> Validate(T t);    
}