using System.Text.RegularExpressions;

namespace Jcl.FileBrowser.Utils;

public static class EnumerableUtils
{
    private static IEnumerable<T> OrderByNaturalSortImpl<T>(this IEnumerable<T> source, Func<T, string> selector,
        bool ascending, int? max)
    {
        if (max is null)
        {
            // Jcl: do not enumerate deferred execution multiple times
            source = source.ToArray();
            max = source.SelectMany(i => Regex.Matches(selector(i), @"\d+").Select(m => (int?)m.Value.Length)).Max() ??
                  0;
        }

        string OrderByFn(T i) => Regex.Replace(selector(i), @"\d+",
            m => m.Value.PadLeft(max.Value, '0') + (max.Value - m.Value.Length));

        return ascending ? source.OrderBy(OrderByFn) : source.OrderByDescending(OrderByFn);
    }

    public static IEnumerable<T> OrderByNaturalSort<T>(this IEnumerable<T> source, Func<T, string> selector,
        int? max = null)
    {
        return source.OrderByNaturalSortImpl(selector, true, max);
    }

    public static IEnumerable<T> OrderByNaturalSortDescending<T>(this IEnumerable<T> source, Func<T, string> selector, int? max = null)
    {
        return source.OrderByNaturalSortImpl(selector, false, max);
    }
}