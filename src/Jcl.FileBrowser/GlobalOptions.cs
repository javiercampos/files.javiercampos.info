namespace Jcl.FileBrowser;

public class GlobalOptions
{
    public bool CacheEnabled { get; set; } = true;
    public bool HideDotFiles { get; set; } = true;
    public string BaseFolder { get; set; } = string.Empty;
    public string BaseBrowsingPath { get; set; } = "/";
    public string BaseBrowsingDir => BaseBrowsingPath.StartsWith("/") ? BaseBrowsingPath[1..] : BaseBrowsingPath;
    public string SiteTitle { get; set; } = "files";

    public string BuildPathWithBaseBrowsingPath(string? path)
    {
        path ??= "/";
        if (!path.StartsWith("/")) path = "/" + path;
        path = "/" + BaseBrowsingPath + path;
        if (path.StartsWith("//")) path = path[1..];
        return path;
    }
    public string BuildPathWithSiteTitle(string? path)
    {
        path ??= "/";
        if (!path.StartsWith("/")) path = "/" + path;
        return SiteTitle + path;
    }
}