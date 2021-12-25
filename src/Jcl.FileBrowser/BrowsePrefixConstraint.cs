using Microsoft.Extensions.Options;

namespace Jcl.FileBrowser;

public class BrowsePrefixConstraint : IRouteConstraint
{
    private readonly IOptions<GlobalOptions> _options;
    public const string PrefixName = "browsePrefix";

    public BrowsePrefixConstraint(IOptions<GlobalOptions> options)
    {
        _options = options;
    }

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        return string.Equals(values[routeKey]?.ToString(), _options.Value.BaseBrowsingDir, StringComparison.InvariantCultureIgnoreCase);
    }
}