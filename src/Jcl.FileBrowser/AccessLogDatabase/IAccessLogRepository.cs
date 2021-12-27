namespace Jcl.FileBrowser.AccessLogDatabase;

public interface IAccessLogRepository : IAsyncDisposable
{
    Task LogAccessAsync(AccessLogType type, string route, string? remoteIp);
}