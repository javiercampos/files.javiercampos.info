using Jcl.FileBrowser.AccessLogDatabase;
using Jcl.FileBrowser.Services.CodeViewer;
using Jcl.FileBrowser.Services.MimeTypes;
using Jcl.FileBrowser.Services.RelativePath;
using Jcl.FileBrowser.Services.Thumbnails;
using Jcl.FileBrowser.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Jcl.FileBrowser.Pages;

public enum ViewType
{
    Browser,
    CodeViewer
}

public class IndexModel : PageModel
{
    private readonly IMimeTypeMapper _mimeTypeMapper;
    private readonly IRelativePathManager _relativePathManager;
    private readonly ICodeViewerManager _codeViewerManager;
    private readonly IAccessLogRepository _accessLogRepository;
    private readonly IThumbnailBuilder _thumbnailBuilder;
    private readonly IOptions<GlobalOptions> _options;
    private readonly IWebHostEnvironment _environment;

    private string? _route;

    public ViewType ViewType { get; private set; }

    public string? BrowserSort { get; set; }
    
    public string? Route
    {
        get => _route;
        private set
        {
            _route = value;
            ViewData["Title"] = _options.Value.BuildPathWithSiteTitle(_route);
        }
    }

    public string? CodeViewerLanguage { get; private set; }

    public IndexModel(IMimeTypeMapper mimeTypeMapper, IRelativePathManager relativePathManager,
        ICodeViewerManager codeViewerManager, IAccessLogRepository accessLogRepository,
        IThumbnailBuilder thumbnailBuilder, IOptions<GlobalOptions> options, IWebHostEnvironment environment)
    {
        _mimeTypeMapper = mimeTypeMapper;
        _relativePathManager = relativePathManager;
        _codeViewerManager = codeViewerManager;
        _accessLogRepository = accessLogRepository;
        _thumbnailBuilder = thumbnailBuilder;
        _options = options;
        _environment = environment;
    }

    public async Task<IActionResult> OnGetAsync(string? route)
    {
        route ??= "/";

        if (!string.IsNullOrWhiteSpace(_options.Value.BaseBrowsingPath))
        {
            if (!route.StartsWith(_options.Value.BaseBrowsingDir))
            {
                // Jcl: Do not send 301's on development
                return _environment.IsDevelopment() 
                            ? Redirect(_options.Value.BuildPathWithBaseBrowsingPath(route)) 
                            : RedirectPermanent(_options.Value.BuildPathWithBaseBrowsingPath(route));
            }
            route = route[_options.Value.BaseBrowsingDir.Length..];
        }
        
        _relativePathManager.ThrowIfInvalidPath(route);
        var fullPath = _relativePathManager.GetFullInternalPath(route);

        return System.IO.File.Exists(fullPath)
            ? IsThumbnailRequest()
                ? await ManageThumbnailRequestAsync(fullPath)
                : await ManageFileAsync(route, fullPath)
            : await ManageDirectoryAsync(route, fullPath);
    }


    private async Task<IActionResult> ManageDirectoryAsync(string route, string fullPath)
    {
        if (!Directory.Exists(fullPath))
        {
            await _accessLogRepository.LogAccessAsync(AccessLogType.NotFound, route, GetRemoteIp());
            return NotFound();
        }

        string? sortOrder = null;
        if(Request.Query.TryGetValue("s", out var sortOrderQueryValue))
        {
            sortOrder = sortOrderQueryValue;
        }

        var eTagResponse = GetDirectoryCacheTag(fullPath, sortOrder);
        if (eTagResponse is not null && Request.IsRequestCached(eTagResponse))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        ViewType = ViewType.Browser;
        Route = route;
        BrowserSort = sortOrder;
        await _accessLogRepository.LogAccessAsync(AccessLogType.Browse, route, GetRemoteIp());

        if (eTagResponse is not null)
        {
            Response.AddEtagHeader(eTagResponse, true);
        }

        return Page();
    }

    private async Task<IActionResult> ManageFileAsync(string route, string fullPath)
    {
        if (!System.IO.File.Exists(fullPath))
        {
            await _accessLogRepository.LogAccessAsync(AccessLogType.NotFound, route, GetRemoteIp());
            return NotFound();
        }

        var fileCacheTag = GetFileCacheTag(fullPath, out var lastModified);
        if (fileCacheTag is not null && Request.IsRequestCached(fileCacheTag))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        var showViewer = false;
        if (!IsDownloadRequest() && Path.HasExtension(fullPath))
        {
            var language = _codeViewerManager.GetLanguage(fullPath);
            if (language is not null)
            {
                ViewType = ViewType.CodeViewer;
                Route = route;
                CodeViewerLanguage = language;
                showViewer = true;
            }
        }

        await _accessLogRepository.LogAccessAsync(showViewer ? AccessLogType.View : AccessLogType.Download, route,
            GetRemoteIp());
        if (fileCacheTag is not null)
        {
            Response.AddEtagHeader(fileCacheTag, true, lastModified);
        }

        if (showViewer)
        {
            return Page();
        }

        var stream = System.IO.File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var fileNameHint = ForceDownload() ? Path.GetFileName(fullPath) : null;
        var mimeType = _mimeTypeMapper.Map(fullPath);
        // Jcl: Pass the file name if we have a query string of "download". Browser's will get hinted you want to actually download it  
        return File(stream, mimeType, fileNameHint);
    }

    private async Task<IActionResult> ManageThumbnailRequestAsync(string fullPath)
    {
        var width = 32;
        var height = 32;
        if (Request.Query.TryGetValue("w", out var widthStr)) width = Convert.ToInt32(widthStr);
        if (Request.Query.TryGetValue("h", out var heightStr)) height = Convert.ToInt32(heightStr);

        if (!System.IO.File.Exists(fullPath))
        {
            return NotFound();
        }

        var fi = new FileInfo(fullPath);
        var checkSum = _thumbnailBuilder.BuildThumbnailCacheTag(fi.LastWriteTimeUtc, width, height);
        if (Request.IsRequestCached(checkSum))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        var outputStream = await _thumbnailBuilder.BuildThumbnail(width, height, fullPath);
        if (outputStream is null)
        {
            return NotFound();
        }
        Response.AddEtagHeader(checkSum, true);
        Response.AddCacheControl();
        return File(outputStream, "image/png");
    }

    private int? GetFileHashCode(string filepath, out DateTime? lastModified)
    {
        var fi = new FileInfo(filepath);
        lastModified = fi.LastWriteTimeUtc;
        return fi.Exists ? fi.LastWriteTimeUtc.GetHashCode() : null;
    }

    private string? GetFileCacheTag(string filepath, out DateTime? lastModified)
    {
        var fileHash = GetFileHashCode(filepath, out lastModified);
        if (fileHash is not null && lastModified is not null) return fileHash.ToString();
        lastModified = null;
        return null;
    }

    private string? GetDirectoryCacheTag(string fullpath, params object?[] additionalHashes)
    {
        var resultHash = MiscUtils.CombineHashesLong(new long());
        foreach (var f in Directory.EnumerateFiles(fullpath))
        {
            var hash = GetFileHashCode(f, out _);
            if (hash is null)
            {
                return null;
            }

            resultHash = MiscUtils.CombineHashesLong(resultHash, hash);
        }
        if (additionalHashes.Length > 0 && additionalHashes.Any(x => x is not null))
        {
            // Jcl: add OfType to avoid nullability compiler warnings
            resultHash = MiscUtils.CombineHashesLong(resultHash, additionalHashes.Where(x => x is not null).OfType<object>());    
        }
        return resultHash.ToString();
    }

    private bool IsDownloadRequest()
    {
        // Jcl: sec-fetch-dest seems to be "document" in Chrome when opening for navigation, and "empty" when opening for download
        return ForceDownload() ||
               Request.Headers.TryGetValue("sec-fetch-dest", out var fetchDest) && fetchDest != "document";
    }

    private bool ForceDownload() => Request.Query.ContainsKey("download");

    private bool IsThumbnailRequest() => Request.Query.ContainsKey("thumbnail");

    private string? GetRemoteIp() => HttpContext.Connection.RemoteIpAddress?.ToString();
}