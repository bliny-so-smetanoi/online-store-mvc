using Domain.Enums;

namespace Application.UseCases.Orders.Dtos;

public class VisitorOrderDto
{
    public Guid OrderId { get; set; }
    public string Number => OrderId.ToString("N")[..8].ToUpperInvariant();
    public DateTime Created { get; set; }
    public OrderStatus Status { get; set; }

    public string Country { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Street { get; set; } = default!;
    public string PostalCode { get; set; } = default!;

    public string MaskedCardNumber { get; set; } = default!;
    public string CardHolderName { get; set; } = default!;
    public string ExpirationDate { get; set; } = default!;

    public List<CartItemDto> Items { get; set; } = new();
    public int TotalQuantity => Items.Sum(x => x.Quantity);
    public decimal TotalPrice => Items.Sum(x => x.TotalPrice);
}
