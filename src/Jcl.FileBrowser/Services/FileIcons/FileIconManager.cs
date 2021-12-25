using Jcl.FileBrowser.Services.MimeTypes;
using Jcl.FileBrowser.Services.RelativePath;
using Microsoft.Extensions.Options;

namespace Jcl.FileBrowser.Services.FileIcons;

public class FileIconManager : IFileIconManager
{
    private readonly IMimeTypeMapper _mimeTypeMapper;
    private readonly IImageAllowedMimeTypes _imageAllowedMimeTypes;
    private readonly IRelativePathManager _relativePathManager;
    private readonly IOptions<GlobalOptions> _options;

    public FileIconManager(IMimeTypeMapper mimeTypeMapper, IImageAllowedMimeTypes imageAllowedMimeTypes,
        IRelativePathManager relativePathManager, IOptions<GlobalOptions> options)
    {
        _mimeTypeMapper = mimeTypeMapper;
        _imageAllowedMimeTypes = imageAllowedMimeTypes;
        _relativePathManager = relativePathManager;
        _options = options;
    }

    private string BuildIconPath(string? icon)
    {
        var relativeFolder = "img/icons/vivid/";
        if (icon is null) return "/" + relativeFolder + "blank.svg";
        var folder = _relativePathManager.GetWebrootPath(relativeFolder);
        return "/" + relativeFolder +
               (File.Exists(Path.Combine(folder, icon.ToLower() + ".svg")) ? icon.ToLower() : "blank") + ".svg";
    }

    private string GetIconUrlForExtension(string? extension)
    {
        if (extension is not null && extension.StartsWith(".")) extension = extension[1..];
        return BuildIconPath(extension);
    }

    public string GetFileIconUrl(string filePath)
    {
        var fi = new FileInfo(filePath);
        var mimeType = _mimeTypeMapper.Map(fi.Name);

        string iconUrl;
        if (_imageAllowedMimeTypes.IsMimeTypeAllowed(mimeType))
        {
            iconUrl = _options.Value.BuildPathWithBaseBrowsingPath(_relativePathManager.GetRelativePath(fi.FullName) +
                                                                   "?thumbnail&w=32&h=32");
        }
        else
        {
            iconUrl = GetIconUrlForExtension(fi.Extension);
        }

        return iconUrl;
    }
}