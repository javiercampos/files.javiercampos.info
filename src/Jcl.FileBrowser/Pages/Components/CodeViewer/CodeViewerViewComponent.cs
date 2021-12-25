using Jcl.FileBrowser.Services;
using Jcl.FileBrowser.Services.RelativePath;
using Microsoft.AspNetCore.Mvc;

namespace Jcl.FileBrowser.Pages.Components.CodeViewer;

public class CodeViewerViewComponent : ViewComponent
{
    private readonly IRelativePathManager _relativePathManager;

    public CodeViewerViewComponent(IRelativePathManager relativePathManager)
    {
        _relativePathManager = relativePathManager;
    }

    public async Task<IViewComponentResult> InvokeAsync(string route, string language)
    {
        var fullPath = _relativePathManager.GetFullInternalPath(route);
        return View("CodeView", new CodeViewerModel(route, await File.ReadAllTextAsync(fullPath), language));
    }
}