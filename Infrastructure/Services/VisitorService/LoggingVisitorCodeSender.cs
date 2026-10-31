using Application.Services.IVisitorService;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.VisitorService;

// Для разработки: код пишется в лог. Для продакшена замени на отправку SMS.
public sealed class LoggingVisitorCodeSender(ILogger<LoggingVisitorCodeSender> logger) : IVisitorCodeSender
{
    public Task SendAsync(string phone, string code, CancellationToken cancellationToken)
    {
        logger.LogInformation("Visitor authentication code for {Phone}: {Code}", phone, code);
        return Task.CompletedTask;
    }
}
