namespace Jcl.FileBrowser;

public static class ConfigurationChecker
{
    public static void AssertConfiguration(GlobalOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.SiteTitle))
        {
            throw new InvalidOperationException("Invalid site title");
        }
        if (string.IsNullOrWhiteSpace(options.BaseFolder) || !Directory.Exists(options.BaseFolder))
        {
            throw new InvalidOperationException("Invalid base folde: \"" + options.BaseFolder + "\"");
        }
    }
}