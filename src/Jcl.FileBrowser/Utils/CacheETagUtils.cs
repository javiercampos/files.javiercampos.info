using System.Globalization;
using Microsoft.Net.Http.Headers;
using EntityTagHeaderValue = System.Net.Http.Headers.EntityTagHeaderValue;

namespace Jcl.FileBrowser.Utils;

public static class CacheETagUtils
{
    private static GlobalOptions _globalOptions = new GlobalOptions();

    public static void Initialize(GlobalOptions options)
    {
        _globalOptions = options;
    }
    public static void AddEtagHeader(this HttpResponse response, string etag, bool weak = false,
        DateTime? lastModified = null)
    {
        if (!_globalOptions.CacheEnabled) return;
        
        var headerValue = new EntityTagHeaderValue("\"" + etag + "\"", weak);
        response.Headers[HeaderNames.ETag] = headerValue.ToString();
        if (lastModified.HasValue)
        {
            response.AddLastModifiedHeader(lastModified.Value);
        }
    }

    public static void AddLastModifiedHeader(this HttpResponse response, DateTime dateTime)
    {
        if (!_globalOptions.CacheEnabled) return;

        var headerValue = dateTime.ToUniversalTime()
            .ToString("ddd, dd MMM yyyy HH:mm:ss 'UTC'", CultureInfo.InvariantCulture);
        response.Headers[HeaderNames.LastModified] = headerValue;
    }

    public static void AddCacheControl(this HttpResponse response, TimeSpan? maxAge = null, bool mustRevalidate = true,
        bool @private = true)
    {
        if (!_globalOptions.CacheEnabled) return;

        response.Headers.CacheControl = new CacheControlHeaderValue
        {
            MaxAge = maxAge ?? TimeSpan.FromSeconds(60),
            MustRevalidate = mustRevalidate,
            Private = @private
        }.ToString();
    }

    public static bool IsRequestCached(this HttpRequest request, string eTagValue, bool allowWeakMatch = true)
    {
        if (!_globalOptions.CacheEnabled) return false;

        if (!request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var value))
        {
            return false;
        }

        return CheckIfNoneMatch(value, eTagValue, false) 
               || allowWeakMatch && CheckIfNoneMatch(value, eTagValue, true);
    }

    private static bool CheckIfNoneMatch(string ifNoneMatch, string eTagValue, bool weak)
    {
        if (!_globalOptions.CacheEnabled) return false;

        return weak
            ? string.Equals(ifNoneMatch, "W/\"" + eTagValue + "\"", StringComparison.Ordinal)
            : string.Equals(ifNoneMatch, eTagValue, StringComparison.Ordinal);
    }
}