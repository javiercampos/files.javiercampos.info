namespace Jcl.FileBrowser.Pages.Components.Browser;

public record BrowserViewModel(IEnumerable<FsEntry> Entries, string Route, string? Sorting);