namespace Application.UseCases.Orders.Dtos;

public class VisitorCartDto
{
    public Guid VisitorId { get; set; }
    public string Phone { get; set; } = default!;
    public string? Email { get; set; }
    public DateTime RegisteredAt { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public int TotalQuantity => Items.Sum(x => x.Quantity);
    public decimal TotalPrice => Items.Sum(x => x.TotalPrice);
}

public class CartItemDto
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = default!;
    public string? CategoryName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public Guid? ImageId { get; set; }
    public decimal TotalPrice => Price * Quantity;
}
