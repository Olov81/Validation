using System.Reflection;

namespace Validation.Cod;

public record ValidationResult(string PropertyName, bool IsValid, string Description);

public class Validator<TRules>(TRules rules) where TRules : class
{
    public IEnumerable<ValidationResult> Validate(object obj)
    {
        var validatorInfos = rules
            .GetType()
            .GetProperties()
            .Select(p => (PropertyName: p.Name, Validator: p.GetValue(rules), ValidatorType: GetUltimateBaseType(p.PropertyType)))
            .Where(x => x.ValidatorType.GetGenericTypeDefinition() == typeof(PropertyValidator<,>))
            .Select(p => (
                p.PropertyName,
                p.Validator,
                ValidationTargetType: GetValidationTargetType(p.ValidatorType),
                Validate: GetValidationMethod(p.ValidatorType)));

        var targetProperties = obj
            .GetType()
            .GetProperties()
            .Where(p => validatorInfos.Any(v => v.PropertyName == p.Name))
            .ToDictionary(p => p.Name, p => p.GetValue(obj)!);
        
        foreach (var validatorInfo in validatorInfos)
        {
            yield return ValidateCurrent();
            
            ValidationResult ValidateCurrent()
            {
                if (!targetProperties.ContainsKey(validatorInfo.PropertyName))
                {
                    return new ValidationResult(validatorInfo.PropertyName, false, "Has property");
                }

                var value = targetProperties[validatorInfo.PropertyName];

                if (value.GetType() != validatorInfo.ValidationTargetType)
                {
                    return new ValidationResult(validatorInfo.PropertyName, false, $"Is {validatorInfo.ValidationTargetType.Name}");
                }
            
                var isValid = (bool)validatorInfo.Validate.Invoke(validatorInfo.Validator, [targetProperties[validatorInfo.PropertyName]])!;

                return new ValidationResult(validatorInfo.PropertyName, isValid, "Is valid");
            }
        }
    }

    private static Type GetValidationTargetType(Type validatorType)
    {
        var genericArguments = validatorType.GetGenericArguments();
        return genericArguments[0];
    }

    private static MethodInfo GetValidationMethod(Type validatorType)
    {
        return validatorType.GetMethod("Validate", BindingFlags.NonPublic | BindingFlags.Instance)!; 
    }

    private static Type GetUltimateBaseType(Type t)
    {
        var baseType = t.BaseType;
        return baseType == typeof(object) ? t : GetUltimateBaseType(baseType!);
    }
}

public class PropertyValidator<T, TValidator> where TValidator : PropertyValidator<T, TValidator>
{
    private readonly List<Rule> _rules = [];
    
    public TValidator Required()
    {
        Add(new Rule(x => x != null));
        return This;
    }
    
    private TValidator This => (TValidator)this;
    
    protected record Rule(Func<T, bool> IsValid);
    
    protected void Add(Rule rule)
    {
        _rules.Add(rule);
    }
    
    protected bool Validate(T value)
    {
        return _rules.All(r => r.IsValid(value));
    }
}

public class StringValidator : PropertyValidator<string, StringValidator>
{
    
}

public class IntValidator : PropertyValidator<int, IntValidator>
{
    public IntValidator Min(int minValue)
    {
        Add(new Rule(i => i >= minValue));
        return this;
    }
    
    public IntValidator Max(int maxValue)
    {
        Add(new Rule(i => i <= maxValue));
        return this;
    }
}

public static class Cod
{
    public static Validator<TRules> Create<TRules>(TRules rules) where TRules : class
    {
        return new Validator<TRules>(rules);
    }
    
    public static StringValidator String()
    {
        return new StringValidator();
    }

    public static IntValidator Int()
    {
        return new IntValidator();
    }
}