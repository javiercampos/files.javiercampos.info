namespace Jcl.FileBrowser.Services.Thumbnails;

public static class ThumbnailBuilderExtensions
{
    public static IServiceCollection AddThumbnailBuilder(this IServiceCollection services)
    {
        return services.AddSingleton<IThumbnailBuilder, ThumbnailBuilder>();
    }
}