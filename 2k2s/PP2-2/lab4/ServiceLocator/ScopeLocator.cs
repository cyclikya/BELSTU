using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ServiceLocator
{
    public static class ScopeLocator
    {
        private static readonly Dictionary<Type, Func<IServiceProvider, object>> _services = new();
        private static IServiceScope? _currentScope;

        public static void Register<T>(Func<IServiceProvider, T> factory) where T : class
        {
            _services[typeof(T)] = sp => factory(sp);
        }

        public static T Resolve<T>() where T : class
        {
            return _currentScope.ServiceProvider.GetService<T>();
        }

        public static IServiceScope BeginScope()
        {
            var newScope = new ServiceScope(_services);
            _currentScope = newScope;
            return newScope;
        }

        public static void EndCurrentScope()
        {
            _currentScope?.Dispose();
            _currentScope = null;
        }
    }
}