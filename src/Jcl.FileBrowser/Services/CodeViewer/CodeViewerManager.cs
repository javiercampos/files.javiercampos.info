namespace Jcl.FileBrowser.Services.CodeViewer;

public class CodeViewerManager : ICodeViewerManager
{
    // Jcl: when the value is null, key is used as value
    private static readonly Dictionary<string, string?> CodeViewerExtensionLanguages = new()
    {
        { "txt", "plaintext" },
        { "cs", "csharp" },
        { "cpp", null },
        { "c", null },
        { "h", "c" },
        { "m", "objective-c" },
        { "html", null },
        { "js", "javascript" },
        { "ts", "typescript" },
        { "json", null },
        { "xml", null },
        { "css", null },
        { "less", null },
        { "scss", null },
        { "sass", "scss" },
        { "go", null },
        { "pl", "perl" },
        { "md", "markdown" },
        { "sql", null },
        { "lua", null },
        { "rb", "ruby" },
        { "yaml", null },
        { "swift", null },
        { "ini", null },
        { "py", "python" },
        { "vb", "vbnet" },
        { "php", null },
        { "htaccess", "apacheconf" }
    };

    public string? GetLanguage(string fileLocalPath)
    {
        var ext = Path.GetExtension(fileLocalPath);
        if (ext.StartsWith(".")) ext = ext[1..];
        if (CodeViewerExtensionLanguages.ContainsKey(ext))
        {
            return CodeViewerExtensionLanguages[ext] ?? ext;
        }
        return null;
    }
}