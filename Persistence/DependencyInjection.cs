using Application.Repositories.ICategoryRepository;
using Application.Repositories.IProductRepository;
using Application.Repositories.ITodoItemRepository;
using Application.Repositories.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using Persistence.Repositories.CategoryRepository;
using Persistence.Repositories.Orders;
using Persistence.Repositories.ProductRepository;
using Persistence.Repositories.TodoItemRepository;

namespace Persistence;

public static class DependencyInjection
{
    public static void AddPersistance(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        AddRepositories(services);
    }
    
    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITodoItemRepository, TodoItemRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IVisitorRepository, VisitorRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
    }
}
