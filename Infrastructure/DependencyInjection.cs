using Application.Services.ITodoItemService;
using Application.Services.IVisitorService;
using Infrastructure.Services.TodoItemService;
using Infrastructure.Services.VisitorService;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddServices();
    }
    
    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITodoItemService, TodoItemService>();
        services.AddScoped<ILocalVisitorAuthService, LocalVisitorAuthService>();
        services.AddScoped<IVisitorCodeSender, LoggingVisitorCodeSender>();
        services.AddScoped<IVisitorLoginService, VisitorLoginService>();
    }
}