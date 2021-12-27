namespace Jcl.FileBrowser.AccessLogDatabase;

public interface IAccessLogRepositoryInitialization : IAsyncDisposable
{
    Task InitializeAsync();
}