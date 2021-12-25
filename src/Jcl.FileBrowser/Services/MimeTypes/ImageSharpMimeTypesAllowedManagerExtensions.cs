namespace Jcl.FileBrowser.Services.MimeTypes;

public static class ImageSharpMimeTypesAllowedManagerExtensions
{
    public static IServiceCollection AddImageSharpMimeTypesAllowedManager(this IServiceCollection services)
    {
        return services.AddSingleton<IImageAllowedMimeTypes, ImageSharpMimeTypesAllowed>();
    }
}