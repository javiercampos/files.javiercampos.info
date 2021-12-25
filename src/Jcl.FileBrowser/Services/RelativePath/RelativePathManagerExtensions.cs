namespace Jcl.FileBrowser.Services.RelativePath;

public static class RelativePathManagerExtensions
{
    public static IServiceCollection AddRelativePathManager(this IServiceCollection services)
    {
        return services.AddSingleton<IRelativePathManager, RelativePathManager>();
    }
}