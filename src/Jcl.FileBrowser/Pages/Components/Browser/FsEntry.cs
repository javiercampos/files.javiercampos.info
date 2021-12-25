namespace Jcl.FileBrowser.Pages.Components.Browser;

public record FsEntry(string Path, string DisplayName, string? IconUrl, long? FileSize, DateTime LastModified);