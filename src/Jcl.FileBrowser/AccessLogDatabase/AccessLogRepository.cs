using Microsoft.Extensions.Options;
using SQLite;

namespace Jcl.FileBrowser.AccessLogDatabase;

public sealed class AccessLogRepository : IAccessLogRepository, IAccessLogRepositoryInitialization
{
    private readonly IOptions<AccessLogOptions> _options;
    private readonly ILogger<AccessLogRepository> _logger;
    private readonly SQLiteAsyncConnection? _connection;
    private int _initialized;
    
    private static readonly SemaphoreSlim InitializationSemaphore = new(1, 1);

    public AccessLogRepository(IOptions<AccessLogOptions> options, ILogger<AccessLogRepository> logger)
    {
        _options = options;
        _logger = logger;
        if (options.Value.Enabled)
        {
            _logger.LogTrace("Initializing SQLite Connection to {Path}", _options.Value.DbPath);
            _connection = new SQLiteAsyncConnection(_options.Value.DbPath);    
        }
    }

    public async Task InitializeAsync()
    {
        if (!_options.Value.Enabled) return;
        if (_connection == null) throw new InvalidOperationException("No available SQLite connection");
        await InitializationSemaphore.WaitAsync();
        try
        {
            if (Interlocked.Exchange(ref _initialized, 1) == 0)
            {
                _logger.LogInformation("Migrating SQLite database {Path}", _options.Value.DbPath);
                await _connection.CreateTableAsync(typeof(AccessLog));
            }
        }
        catch
        {
            Interlocked.Exchange(ref _initialized, 0);
            throw;
        }
        finally
        {
            InitializationSemaphore.Release();
        }
    }

    public async Task LogAccessAsync(AccessLogType type, string route, string? remoteIp)
    {
        if (!_options.Value.Enabled) return;
        if (_connection == null) throw new InvalidOperationException("No available SQLite connection");
        var accessLog = new AccessLog(type.ToString(), route, remoteIp);
        await _connection.InsertAsync(accessLog);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.CloseAsync();    
        }
        
    }
}