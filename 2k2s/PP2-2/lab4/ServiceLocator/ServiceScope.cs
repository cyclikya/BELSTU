using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace ServiceLocator
{
    public class ServiceScope : IServiceScope //нужен для создания разных видов сервисов
    {
        private readonly Dictionary<Type, object> _scoped = new();
        public IServiceProvider ServiceProvider { get; }

        public ServiceScope(Dictionary<Type, Func<IServiceProvider, object>> services)
        {
            ServiceProvider = new ServiceProvider(services, _scoped);
        }

        public void Dispose()
        {
            _scoped.Clear();
        }
    }
}