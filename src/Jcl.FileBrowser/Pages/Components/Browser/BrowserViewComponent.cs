using Jcl.FileBrowser.Services;
using Jcl.FileBrowser.Services.FileIcons;
using Jcl.FileBrowser.Services.MimeTypes;
using Jcl.FileBrowser.Services.RelativePath;
using Jcl.FileBrowser.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Jcl.FileBrowser.Pages.Components.Browser;

public class BrowserViewComponent : ViewComponent
{
    private readonly IOptions<GlobalOptions> _options;
    private readonly IFileIconManager _fileIconManager;
    private readonly IRelativePathManager _relativePathManager;

    public BrowserViewComponent(IConfiguration config, IFileIconManager fileIconManager,
        IRelativePathManager relativePathManager,
        IOptions<GlobalOptions> options)
    {
        _options = options;
        _fileIconManager = fileIconManager;
        _relativePathManager = relativePathManager;
    }

    private FsEntry BuildDirectoryEntry(string directoryPath, string? displayName = null,
        string icon = "folder-open")
    {
        var di = new DirectoryInfo(directoryPath);
        var entry = new FsEntry(_options.Value.BuildPathWithBaseBrowsingPath(_relativePathManager.GetRelativePath(di.FullName)), displayName ?? di.Name,
            "/img/icons/extra/" + icon + ".svg", null, di.LastWriteTimeUtc);
        return entry;
    }

    private FsEntry BuildFileEntry(string filePath)
    {
        var fi = new FileInfo(filePath);
        var entry = new FsEntry(_options.Value.BuildPathWithBaseBrowsingPath(_relativePathManager.GetRelativePath(fi.FullName)), fi.Name,
            _fileIconManager.GetFileIconUrl(filePath), fi.Length, fi.LastWriteTimeUtc);
        return entry;
    }


    public IEnumerable<FsEntry> ReadFiles(string directoryBasePath)
    {
        var nlist = new List<FsEntry>();
        var enumerationOptions = new EnumerationOptions()
        {
            IgnoreInaccessible = true, 
            RecurseSubdirectories = false, 
            MatchCasing = MatchCasing.PlatformDefault,
            AttributesToSkip = FileAttributes.System,
        };

        nlist.AddRange(Directory.EnumerateDirectories(directoryBasePath, "*", enumerationOptions)
            .Select(x => BuildDirectoryEntry(x)));
        var files = Directory.EnumerateFiles(directoryBasePath, "*", enumerationOptions);
        if (_options.Value.HideDotFiles)
        {
            files = files.Where(x => !Path.GetFileName(x).StartsWith("."));
        }
        nlist.AddRange(files.Select(BuildFileEntry));
        return nlist;
    }

    public IViewComponentResult Invoke(string route, string? sorting)
    {
        var fullPath = _relativePathManager.GetFullInternalPath(route);

        var nList = new List<FsEntry>(1);
        if (route != "/")
        {
            var parent = Directory.GetParent(fullPath);
            if (parent is not null)
            {
                nList.Add(BuildDirectoryEntry(parent.FullName, "<parent directory>", "folder-hidden-open"));
            }
        }

        var files = ReadFiles(fullPath);
        
        if (sorting is not null && sorting.Length == 2)
        {
            files = sorting[0] switch
            {
                'A' => sorting[1] switch
                {
                    'N' => files.OrderByNaturalSort(x => x.DisplayName),
                    'L' => files.OrderBy(x => x.LastModified),
                    'S' => files.OrderBy(x => x.FileSize),
                    _ => files
                },
                'D' => sorting[1] switch
                {
                    'N' => files.OrderByNaturalSortDescending(x => x.DisplayName),
                    'L' => files.OrderByDescending(x => x.LastModified),
                    'S' => files.OrderByDescending(x => x.FileSize),
                    _ => files
                },
                _ => files
            };
        }
        var model = new BrowserViewModel(nList.Concat(files), route, sorting);
        return View("BrowserView", model);
    }
}