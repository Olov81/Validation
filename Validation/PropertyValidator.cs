namespace Validation;

public class PropertyValidator<T, TProperty> : IValidator<T>
{
    private readonly Func<T, TProperty> _getProperty;
    private readonly string _propertyName;
    private readonly List<IValidator<TProperty>> _validators = new();

    public PropertyValidator(Func<T, TProperty> getProperty, string propertyName)
    {
        _getProperty = getProperty;
        _propertyName = propertyName;
    }

    public PropertyValidator<T, TProperty> Use(Func<TProperty, bool> rule, string description)
    {
        _validators.Add(new SingleRuleValidator(rule, description));
        return this;
    }
        
    public PropertyValidator<T, TProperty> Use(IValidator<TProperty> validator)
    {
        _validators.Add(validator);
        return this;
    }

    public IEnumerable<ValidationResult> Validate(T t)
    {
        var value = _getProperty(t);
            
        return _validators
            .SelectMany(x => x.Validate(value))
            .Select(r =>
            {
                var name = string.IsNullOrEmpty(r.PropertyName) ? _propertyName : $"{_propertyName}.{r.PropertyName}";
                return new ValidationResult(name, r.IsValid, r.Description);
            });
    }
        
    private class SingleRuleValidator : IValidator<TProperty>
    {
        private readonly Func<TProperty, bool> _rule;
        private readonly string _description;

        public SingleRuleValidator(Func<TProperty, bool> rule, string description)
        {
            _rule = rule;
            _description = description;
        }
            
        public IEnumerable<ValidationResult> Validate(TProperty t)
        {
            return new[] { new ValidationResult(string.Empty, _rule(t), _description) };
        }
    }
}