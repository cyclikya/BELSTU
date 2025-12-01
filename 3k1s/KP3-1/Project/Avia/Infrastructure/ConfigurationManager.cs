using Microsoft.Extensions.Configuration;

namespace Avia.Infrastructure;

public class AppConfigurationManager
{
    private static IConfiguration? _configuration;

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static string GetConnectionString(string name = "DefaultConnection")
    {
        if (_configuration == null)
            throw new InvalidOperationException("Configuration not initialized");

        return _configuration.GetConnectionString(name) 
            ?? throw new InvalidOperationException($"Connection string '{name}' not found");
    }
}

