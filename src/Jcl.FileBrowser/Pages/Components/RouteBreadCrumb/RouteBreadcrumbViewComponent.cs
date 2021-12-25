using Jcl.FileBrowser.Services;
using Jcl.FileBrowser.Services.FileIcons;
using Jcl.FileBrowser.Services.RelativePath;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Jcl.FileBrowser.Pages.Components.RouteBreadCrumb;

public record RouteBreadcrumbParameters(string? Route, bool AllowDownload = false);

public class RouteBreadcrumbViewComponent : ViewComponent
{
    private readonly IRelativePathManager _relativePathManager;
    private readonly IFileIconManager _fileIconManager;
    private readonly IOptions<GlobalOptions> _options;

    public RouteBreadcrumbViewComponent(IRelativePathManager relativePathManager, IFileIconManager fileIconManager,
        IOptions<GlobalOptions> options)
    {
        _relativePathManager = relativePathManager;
        _fileIconManager = fileIconManager;
        _options = options;
    }

    private RoutePart[] BuildRouteSplit(string? fullPath, bool allowDownload = false)
    {
        if (fullPath is null)
        {
            throw new InvalidOperationException();
        }

        var isFile = File.Exists(fullPath);
        var routes = _relativePathManager.GetRelativePath(fullPath).Split(
            new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
            StringSplitOptions.RemoveEmptyEntries);
        if (routes.Length == 1 && routes[0] == ".") routes = Array.Empty<string>();

        var result = new List<RoutePart>();
        var aggregateRoute = "";
        result.Add(new RoutePart(_options.Value.SiteTitle, _options.Value.BuildPathWithBaseBrowsingPath("/"), null));
        foreach (var route in routes.SkipLast(isFile ? 1 : 0))
        {
            aggregateRoute += "/" + route;
            result.Add(new RoutePart(route, _options.Value.BuildPathWithBaseBrowsingPath(aggregateRoute), null));
        }

        if (isFile)
        {
            var route = routes.Last();
            aggregateRoute += "/" + route + "?download";
            result.Add(new RoutePart(route, allowDownload ? _options.Value.BuildPathWithBaseBrowsingPath(aggregateRoute) : null,
                _fileIconManager.GetFileIconUrl(fullPath)));
        }

        return result.ToArray();
    }

    public IViewComponentResult Invoke(RouteBreadcrumbParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters.Route);
        var fullPath = _relativePathManager.GetFullInternalPath(parameters.Route);
        var routes = BuildRouteSplit(fullPath, parameters.AllowDownload);
        return View("RouteBreadcrumb", new RouteBreadcrumbModel(routes));
    }
}