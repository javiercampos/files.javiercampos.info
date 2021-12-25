namespace Jcl.FileBrowser.AccessLogDatabase;

public static class AccessLogRepositoryExtensions
{
    public static IServiceCollection AddAccessLogRepository(this IServiceCollection services)
    {
        return services.AddScoped<IAccessLogRepository, AccessLogRepository>();
    }
}