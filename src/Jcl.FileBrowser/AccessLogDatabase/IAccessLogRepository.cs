namespace Jcl.FileBrowser.AccessLogDatabase;

public interface IAccessLogRepository
{
    Task LogAccessAsync(AccessLogType type, string route, string? remoteIp);
}