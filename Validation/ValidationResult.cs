namespace Validation;

public class ValidationResult
{
    public ValidationResult(string propertyName, bool isValid, string description)
    {
        PropertyName = propertyName;
        IsValid = isValid;
        Description = description;
    }

    public string PropertyName { get; }
    public bool IsValid { get; }
    public string Description { get; }
}