using Validation;
using Validation.Cod;
using Xunit;

namespace ValidationTests;



public class Tests
{
    [Fact]
    public void Cepa()
    {
        var validator = Cod.Create(new
        {
            Name = Cod.String().Required(),
            Age = Cod.Int().Required().Min(10).Max(100),
            Apa = Cod.String().Required()
        });
        
        var myDto = new { Name = "Bosse", Age = 5 };

        var results = validator.Validate(myDto).ToList();
    }
    
    [Fact]
    public void Bepa()
    {
        var validator = ValidatorBuilder<MyEditObject>.Create(b => [
            b.Require(x => x.Name).MinLength(1).Email(),
            b.Require(x => x.IntValue).MinValue(10).MaxValue(100),
            b.Value(x => x.LongValue).Range(1, 10),
            b.Value(x => x.DoubleValue).MinValue(10).Use(double.IsFinite, "Finite"),
            b.Require(x => x.Emails).Where(e => e.Email()),
            b.Require(x => x.Person).Use(p => [
                p.Require(x => x.FirstName),
                p.Require(x => x.Address).Use(a => [
                    a.Require(x => x.City),
                    a.Require(x => x.PostalCode)
                ])
            ])
        ]);
        
        var result = validator.Validate(new MyEditObject
        {
            Name = "apa@bepa.com",
            IntValue = 15,
            Person = new NestedObjectClass
            {
                SirName = "Bosse",
                FirstName = "Larsson",
                Address = new Address
                {
                    City = "Ja hej du",
                }
            }
        }).ToList();
    }
    
    [Fact]
    public void Apa()
    {
        var validator = ValidatorBuilder<MyEditObject>.Create(
            b => b.Require(x => x.Name).MinLength(1).Email(),
            b => b.Require(x => x.IntValue).MinValue(10).MaxValue(100),
            b => b.Value(x => x.LongValue).Range(1, 10),
            b => b.Value(x => x.DoubleValue).MinValue(10).Use(double.IsFinite, "Finite"),
            b => b.Require(x => x.Person).Use(
                apa => apa.Require(x => x.SirName),
                apa => apa.Require(x => x.FirstName),
                apa => apa.Optional(x => x.Address).Use(
                    bepa => bepa.Require(x => x.City),
                    bepa => bepa.Require(x => x.PostalCode))));
            
        var result = validator.Validate(new MyEditObject
        {
            Name = "apa@bepa.com",
            IntValue = 15,
            Person = new NestedObjectClass
            {
                SirName = "Bosse",
                FirstName = "Larsson",
                Address = new Address
                {
                    City = "Ja hej du",
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
            b => b.Optional(x => x.Person).Use(nestedValidator));
    }
    
    
    private class Address
    {
        public string? City { get; init; }
        public string? PostalCode { get; init; }    
    }
        
    private class NestedObjectClass
    {
        public string? SirName { get; init; }
        public string? FirstName { get; init; }
            
        public Address? Address { get; init; }
    }
        
    private class MyEditObject
    {
        public string? Name { get; init; }
        public int? IntValue { get; init; }
        public int LongValue { get; init; }
        public double DoubleValue { get; init; }
            
        public NestedObjectClass? Person { get; init; }
        
        public string[]? Emails { get; init; }
    }
}
