namespace Jcl.FileBrowser.Services.CodeViewer;

public interface ICodeViewerManager
{
    string? GetLanguage(string fileLocalPath);
}