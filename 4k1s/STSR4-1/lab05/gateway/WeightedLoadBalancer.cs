using Ocelot.LoadBalancer.Interfaces;
using Ocelot.Responses;
using Ocelot.Values;

public class WeightedLoadBalancer : ILoadBalancer
{
    //50% серверу X, 30% серверу Y, 20% серверу Z
    private static readonly int[] Plan = { 0, 0, 0, 0, 0, 1, 1, 1, 2, 2 };

    private readonly List<ServiceHostAndPort> _hosts;
    private int _counter = -1;

    public WeightedLoadBalancer(List<ServiceHostAndPort> hosts)
    {
        _hosts = hosts;
    }

    public string Type => nameof(WeightedLoadBalancer);

    public Task<Response<ServiceHostAndPort>> LeaseAsync(HttpContext httpContext)
    {
        var step = Interlocked.Increment(ref _counter);
        var host = _hosts[Plan[step % Plan.Length]];
        return Task.FromResult<Response<ServiceHostAndPort>>(new OkResponse<ServiceHostAndPort>(host));
    }

    public void Release(ServiceHostAndPort hostAndPort)
    {
    }
}
