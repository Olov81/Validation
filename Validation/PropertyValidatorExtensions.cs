using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Validation;

public static class PropertyValidatorExtensions
{
    public static PropertyValidator<T, string> MinLength<T>(
        this PropertyValidator<T, string> validator, int minLength)
    {
        return validator.Use(s => s.Length >= minLength, $"MinLength({minLength})");
    }

    public static PropertyValidator<T, string> Email<T>(this PropertyValidator<T, string> validator)
    {
        var attribute = new EmailAddressAttribute();
        return validator.Use(s => attribute.IsValid(s), attribute.GetDataTypeName());
    }
    
    public static PropertyValidator<T, TProperty> MinValue<T, TProperty>(
        this PropertyValidator<T, TProperty> validator, TProperty minValue)
        where TProperty : IComparable<TProperty>
    {
        return validator.Use(i => i.CompareTo(minValue) >= 0, $"MinValue({minValue})");
    }

    public static PropertyValidator<T, TProperty> MaxValue<T, TProperty>(
        this PropertyValidator<T, TProperty> validator, TProperty maxValue) 
        where TProperty : IComparable<TProperty>
    {
        return validator.Use(i => i.CompareTo(maxValue) <= 0, $"MaxValue({maxValue})");
    }
        
    public static PropertyValidator<T, TProperty> Range<T, TProperty>(
        this PropertyValidator<T, TProperty> validator, TProperty minValue, TProperty maxValue) 
        where TProperty : IComparable<TProperty>
    {
        validator.MinValue(minValue);
        return validator.MinValue(maxValue);
    }
        
    public static PropertyValidator<T, TProperty[]> Where<T, TProperty>(
        this PropertyValidator<T, TProperty[]> validator, Func<PropertyValidator<T, TProperty>, PropertyValidator<T, TProperty>> configure) 
        where TProperty : IComparable<TProperty>
    {
        return validator;
    }
    
    public static PropertyValidator<T, TProperty> Use<T, TProperty>(
        this PropertyValidator<T, TProperty> validator, params Action<ValidatorBuilder<TProperty>>[] configure) 
        where TProperty : class
    {
        return validator.Use(ValidatorBuilder<TProperty>.Create(configure));
    }
    
    public static PropertyValidator<T, TProperty> Use<T, TProperty>(
        this PropertyValidator<T, TProperty> validator,
        Func<ValidatorBuilder<TProperty>, IValidator<TProperty>[]> configure)
    {
        return validator.Use(ValidatorBuilder<TProperty>.Create(configure));
    }
}