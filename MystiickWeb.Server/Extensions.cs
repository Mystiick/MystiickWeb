using System.Reflection;

using MystiickWeb.Core;

namespace MystiickWeb.Server.Extensions;

public static class Extensions
{
    /// <summary>
    /// Adds classes with <see cref="InjectableAttribute"/> to the Services collection.
    /// </summary>
    /// <param name="services"></param>
    public static void AddInjectables(this IServiceCollection services)
    {
        foreach (Type service in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => x.IsDefined(typeof(InjectableAttribute), false)))
        {
            var attr = service.GetCustomAttribute<InjectableAttribute>();

            if (attr == null)
                return;

            switch (attr.Setting)
            {
                case InjectableAttribute.InjectableSetting.Scoped:
                    services.AddScoped(attr.Interface, service);
                    break;
                case InjectableAttribute.InjectableSetting.Transient:
                    services.AddTransient(attr.Interface, service);
                    break;
                case InjectableAttribute.InjectableSetting.Singleton:
                    services.AddSingleton(attr.Interface, service);
                    break;
            }

        }
    }
}
