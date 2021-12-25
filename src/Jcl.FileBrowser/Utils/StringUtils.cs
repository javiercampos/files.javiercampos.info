namespace Jcl.FileBrowser.Utils;

public static class StringUtils
{
    public static string ReplacePathSeparator(this string value) =>
        value.Replace('\\', '/');

    public static bool StartsWithAny(this string value, params char[] startsWith) => startsWith.Any(value.StartsWith);
    public static bool StartsWithAny(this string value, params string[] startsWith) => startsWith.Any(value.StartsWith);
}