using Microsoft.AspNetCore.StaticFiles;

namespace Jcl.FileBrowser.Services.MimeTypes;

public class MimeTypeMapper : IMimeTypeMapper
{
    private readonly FileExtensionContentTypeProvider _contentTypeProvider;

    public MimeTypeMapper(FileExtensionContentTypeProvider contentTypeProvider)
    {
        _contentTypeProvider = contentTypeProvider;
    }

    public string Map(string fileName)
    {
        if (!_contentTypeProvider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }
    public string MapExtension(string extension)
    {
        if (!extension.StartsWith(".")) extension = "." + extension;
        return Map(extension);
    }
}