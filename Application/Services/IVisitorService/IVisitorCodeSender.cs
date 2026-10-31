namespace Application.Services.IVisitorService;

public interface IVisitorCodeSender
{
    Task SendAsync(string phone, string code, CancellationToken cancellationToken);
}
