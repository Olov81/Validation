using System.Linq.Expressions;

namespace Validation;

public class PropertyValidator<T, TProperty> : IValidator<T>
{
    private readonly Expression<Func<T, TProperty>> _getProperty;
    private readonly List<IValidator<TProperty>> _validators = new();

    public PropertyValidator(Expression<Func<T, TProperty>> getProperty)
    {
        _getProperty = getProperty;
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
        var value = _getProperty.Compile()(t);
        var propertyName = PropertyHelper.GetPropertyName(_getProperty);
            
        return _validators
            .SelectMany(x => x.Validate(value))
            .Select(r =>
            {
                var name = string.IsNullOrEmpty(r.PropertyName) ? propertyName : $"{propertyName}.{r.PropertyName}";
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