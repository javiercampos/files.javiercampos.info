namespace Jcl.FileBrowser.AccessLogDatabase;

public static class AccessLogRepositoryExtensions
{
    public static IServiceCollection AddAccessLogRepository(this IServiceCollection services)
    {
        services.AddSingleton<IAccessLogRepositoryInitialization, AccessLogRepository>();
        return services.AddScoped<IAccessLogRepository, AccessLogRepository>();
        
    }
}