using SixLabors.ImageSharp;

namespace Jcl.FileBrowser.Services.MimeTypes;

public class ImageSharpMimeTypesAllowed : IImageAllowedMimeTypes
{
    private string[] AllowedMimeTypes { get; }

    public bool IsMimeTypeAllowed(string mimeType) => AllowedMimeTypes.Contains(mimeType);

    public ImageSharpMimeTypesAllowed()
    {
        var config = Configuration.Default;
        AllowedMimeTypes = config.ImageFormats.SelectMany(x => x.MimeTypes).Distinct().ToArray();
    }
}