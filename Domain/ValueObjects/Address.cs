using CSharpFunctionalExtensions;

namespace Domain.ValueObjects;

public class Address : ValueObject
{
    public string Country { get; protected set; }
    public string City { get; protected set; }
    public string Street { get; protected set; }
    public string PostalCode { get; protected set; }

    protected Address() { }
    
    public Address(string country, string city, string street, string postalCode)
    {
        Country = country;
        City = city;
        Street = street;
        PostalCode = postalCode;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return $"{Country} {City} {Street} {PostalCode}";
    }
}