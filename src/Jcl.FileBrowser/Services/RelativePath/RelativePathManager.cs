using Jcl.FileBrowser.Utils;
using Microsoft.Extensions.Options;

namespace Jcl.FileBrowser.Services.RelativePath;

public class RelativePathManager : IRelativePathManager
{
    private readonly IWebHostEnvironment _environment;
    private readonly string _baseFolder;

    public RelativePathManager(IWebHostEnvironment environment, IOptions<GlobalOptions> options)
    {
        _environment = environment;
        _baseFolder = options.Value.BaseFolder;
    }

    public void ThrowIfInvalidPath(string relativeRoute)
    {
        var fullPath = GetFullInternalPath(relativeRoute);
        if (!fullPath.StartsWith(_baseFolder))
        {
            throw new InvalidOperationException("Invalid route");
        }
    }
    
    public string GetFullInternalPath(string relativeRoute)
    {
        if (relativeRoute.StartsWithAny(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
        {
            relativeRoute = relativeRoute[1..];
        }
        return Path.Combine(_baseFolder, relativeRoute).ReplacePathSeparator();
    }
    
    public string GetRelativePath(string fullPath)
    {
        return Path.GetRelativePath(_baseFolder, fullPath).ReplacePathSeparator();
    }

    public string GetWebrootPath(string relativePath)
    {
        return Path.Combine(_environment.WebRootPath, relativePath).ReplacePathSeparator();
    }
}