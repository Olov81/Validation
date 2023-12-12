using Validation;
using Xunit;

namespace ValidationTests;

public class Tests
{
    [Fact]
    public void Apa()
    {
        var validator = ValidatorBuilder<MyEditObject>.Create(
            b => b.Require(x => x.Name).MinLength(1).Email(),
            b => b.Require(x => x.IntValue).MinValue(10).MaxValue(100),
            b => b.Value(x => x.LongValue).Range(1, 10),
            b => b.Value(x => x.DoubleValue).MinValue(10).Use(double.IsFinite, "Finite"),
            b => b.Require(x => x.NestedObject).Use(
                apa => apa.Require(x => x.SirName),
                apa => apa.Require(x => x.FirstName),
                apa => apa.Optional(x => x.AnotherNestedClass).Use(
                    bepa => bepa.Require(x => x.Hej),
                    bepa => bepa.Require(x => x.Hopp))));
            
        var result = validator.Validate(new MyEditObject
        {
            Name = "apa@bepa.com",
            IntValue = 15,
            NestedObject = new NestedObjectClass
            {
                SirName = "Bosse",
                FirstName = "Larsson",
                AnotherNestedClass = new AnotherNestedClass
                {
                    Hej = "Ja hej du",
                }
            }
        }).ToList();
            
        var myObject = new MyEditObject();

        var nestedValidator = ValidatorBuilder<NestedObjectClass>.Create(
            b => b.Optional(x => x.SirName),
            b => b.Optional(x => x.FirstName));
            
        var result2 = myObject.Validate(
            b => b.Optional(x => x.Name).MinLength(1),
            b => b.Optional(x => x.IntValue).MinValue(10).MaxValue(100),
            b => b.Optional(x => x.NestedObject).Use(nestedValidator));
    }
    
    
    private class AnotherNestedClass
    {
        public string? Hej { get; init; }
        public string? Hopp { get; init; }    
    }
        
    private class NestedObjectClass
    {
        public string? SirName { get; init; }
        public string? FirstName { get; init; }
            
        public AnotherNestedClass? AnotherNestedClass { get; init; }
    }
        
    private class MyEditObject
    {
        public string? Name { get; init; }
        public int? IntValue { get; init; }
        public int LongValue { get; init; }
        public double DoubleValue { get; init; }
            
        public NestedObjectClass? NestedObject { get; init; }
    }
}
