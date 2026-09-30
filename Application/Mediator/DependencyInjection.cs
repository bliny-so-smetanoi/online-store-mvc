using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Mediator;

public static class DependencyInjection {
    public static void ConfigureMediator(this IServiceCollection serviceCollection, params Type[] types) {
        serviceCollection.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        serviceCollection.Scan(
            x => {
                foreach (var type in types) {
                    var entryAssembly = type.GetTypeInfo().Assembly;
                    var assemblies = new List<Assembly> { entryAssembly }/*.Concat(referencedAssemblies)*/;

                    x.FromAssemblies(assemblies)
                        .AddClasses(classes => classes.AssignableTo(typeof(IRequestValidator<>)))
                        .AsImplementedInterfaces()
                        .WithScopedLifetime();
                }
            });
        var assemblies = types.Select(x => x.GetTypeInfo().Assembly).Distinct().ToArray();
        serviceCollection.AddMediatR(assemblies);
    }
}