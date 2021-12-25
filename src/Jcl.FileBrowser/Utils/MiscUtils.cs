namespace Jcl.FileBrowser.Utils;

public static class MiscUtils
{
    public static string ToAutomaticFileSize(this long bytes, bool isIso = false)
    {
        var unit = 1024;
        var unitStr = "b";
        if (!isIso) unit = 1000;
        if (bytes < unit) return $"{bytes} {unitStr}";
        
        // unitStr = unitStr.ToUpper();
        if (isIso) unitStr = "i" + unitStr;
        
        var exp = (int)(Math.Log(bytes) / Math.Log(unit));
        return $"{bytes / Math.Pow(unit, exp):##.##} {"KMGTPEZY"[exp - 1]}{unitStr}";
    }
    
    public static long CombineHashesLong(params object[] hashes)
    {
        const long seed = 1009L;
        const long factor = 9176L;
        var hash = seed;
        unchecked
        {
            foreach (var hashObject in hashes)
            {
                hash = hash * factor + hashObject.GetHashCode();
            }
        }
        return hash;
    }        
    
}