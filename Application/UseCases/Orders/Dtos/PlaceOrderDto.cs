namespace Application.UseCases.Orders.Dtos;

public record PlaceOrderDto
{
    public string Country { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;

    public string CardHolderName { get; init; } = string.Empty;
    public string CardNumber { get; init; } = string.Empty;
    // Format MM/YY
    public string ExpirationDate { get; init; } = string.Empty;
    public string Cvv { get; init; } = string.Empty;
}
