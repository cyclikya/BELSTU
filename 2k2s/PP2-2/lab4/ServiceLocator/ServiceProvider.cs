public class ServiceProvider : IServiceProvider
{
    private readonly Dictionary<Type, Func<IServiceProvider, object>> _services;
    private readonly Dictionary<Type, object> _scoped;

    public ServiceProvider(Dictionary<Type, Func<IServiceProvider, object>> services,
                           Dictionary<Type, object> scoped)
    {
        _services = services;
        _scoped = scoped;
    }

    public object GetService(Type serviceType)
    {
        if (!_scoped.TryGetValue(serviceType, out var obj))
        {
            obj = _services[serviceType](this);
            _scoped[serviceType] = obj;
        }
        return obj;
    }
}
