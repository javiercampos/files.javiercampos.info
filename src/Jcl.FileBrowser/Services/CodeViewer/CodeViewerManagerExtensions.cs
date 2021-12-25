namespace Jcl.FileBrowser.Services.CodeViewer;

public static class CodeViewerManagerExtensions
{
    public static IServiceCollection AddCodeViewerManager(this IServiceCollection services)
    {
        return services.AddSingleton<ICodeViewerManager, CodeViewerManager>();
    }
}