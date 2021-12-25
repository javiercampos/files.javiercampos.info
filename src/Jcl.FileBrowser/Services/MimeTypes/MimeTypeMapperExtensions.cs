using Microsoft.AspNetCore.StaticFiles;

namespace Jcl.FileBrowser.Services.MimeTypes;

public static class MimeTypeMapperExtensions
{
    public static IServiceCollection AddMimeTypeMapper(this IServiceCollection services)
    {
        var provider = new FileExtensionContentTypeProvider();
        return services.AddSingleton<IMimeTypeMapper>(new MimeTypeMapper(provider));
    }
}