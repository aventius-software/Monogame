using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace Shared.Extensions;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers all implementations of a given base type or interface as themselves in the DI container.
    /// </summary>
    /// <typeparam name="TBase"></typeparam>
    /// <param name="services"></param>
    /// <param name="lifetime"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static IServiceCollection AddAllImplementationsAsSelf<TBase>(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        Assembly assembly)
    {
        var baseType = typeof(TBase);
        var types = assembly // Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => baseType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

        foreach (var type in types)
        {
            var descriptor = new ServiceDescriptor(type, type, lifetime);
            services.Add(descriptor);
        }

        return services;
    }
}
