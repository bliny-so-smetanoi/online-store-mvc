using Application.Services.IVisitorService;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.VisitorService;

// For development: the code is written to the log. Replace with SMS delivery for production.
public sealed class LoggingVisitorCodeSender(ILogger<LoggingVisitorCodeSender> logger) : IVisitorCodeSender
{
    public Task SendAsync(string phone, string code, CancellationToken cancellationToken)
    {
        logger.LogInformation("Visitor authentication code for {Phone}: {Code}", phone, code);
        return Task.CompletedTask;
    }
}
