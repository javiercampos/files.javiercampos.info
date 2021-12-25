namespace Jcl.FileBrowser.Services.Thumbnails;

public interface IThumbnailBuilder
{
    Task<MemoryStream?> BuildThumbnail(int width, int height, string path);
    string BuildThumbnailCacheTag(DateTime lastModified, int width, int height);
}