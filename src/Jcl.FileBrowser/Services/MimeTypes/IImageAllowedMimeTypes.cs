namespace Jcl.FileBrowser.Services.MimeTypes;

public interface IImageAllowedMimeTypes
{
    bool IsMimeTypeAllowed(string mimeType);
}