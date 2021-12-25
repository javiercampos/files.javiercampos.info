namespace Jcl.FileBrowser.Services.RelativePath;

public interface IRelativePathManager
{
    string GetFullInternalPath(string relativeRoute);
    string GetRelativePath(string fullPath);
    string GetWebrootPath(string relativePath);
    void ThrowIfInvalidPath(string relativeRoute);
}