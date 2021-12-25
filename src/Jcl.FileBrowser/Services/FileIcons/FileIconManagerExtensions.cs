namespace Jcl.FileBrowser.Services.FileIcons;

public static class FileIconManagerExtensions
{
    public static IServiceCollection AddFileIconManager(this IServiceCollection services)
    {
        return services.AddSingleton<IFileIconManager, FileIconManager>();
    }
}