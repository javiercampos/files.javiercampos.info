using System.Text.RegularExpressions;

namespace Jcl.FileBrowser.Utils;

public static class EnumerableUtils
{
    public static IEnumerable<T> OrderByNaturalSort<T>(this IEnumerable<T> source, Func<T, string> selector)
    {
        var max = source.SelectMany(i => Regex.Matches(selector(i), @"\d+").Select(m => (int?)m.Value.Length)).Max() ?? 0;
        return source.OrderBy(i => Regex.Replace(selector(i), @"\d+", m => m.Value.PadLeft(max, '0') + (max - m.Value.Length)));
    }
    public static IEnumerable<T> OrderByNaturalSortDescending<T>(this IEnumerable<T> source, Func<T, string> selector)
    {
        var max = source.SelectMany(i => Regex.Matches(selector(i), @"\d+").Select(m => (int?)m.Value.Length)).Max() ?? 0;
        return source.OrderByDescending(i => Regex.Replace(selector(i), @"\d+", m => m.Value.PadLeft(max, '0') + (max - m.Value.Length)));
    }

}