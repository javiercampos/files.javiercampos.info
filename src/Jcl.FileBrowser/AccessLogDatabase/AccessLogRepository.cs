using Microsoft.Extensions.Options;
using SQLite;

namespace Jcl.FileBrowser.AccessLogDatabase;

public sealed class AccessLogRepository : IAccessLogRepository, IAsyncDisposable
{
    private readonly IOptions<AccessLogOptions> _options;
    private readonly ILogger<AccessLogRepository> _logger;
    private SQLiteAsyncConnection? _connection;
    private bool _initialized;
    
    private static readonly SemaphoreSlim InitializationSemaphore = new(1, 1);

    public AccessLogRepository(IOptions<AccessLogOptions> options, ILogger<AccessLogRepository> logger)
    {
        _options = options;
        _logger = logger;
    }

    private async Task InitializeAsync()
    {
        if (!_options.Value.Enabled) return;
        
        if (_initialized) return;
        await InitializationSemaphore.WaitAsync();
        try
        {
            if (_initialized) return;
            _logger.LogInformation("Initializing SQLite Connection to {Path}", _options.Value.DbPath);
            _connection = new SQLiteAsyncConnection(_options.Value.DbPath);
            _logger.LogInformation("Migrating DB");            
            await _connection.CreateTableAsync(typeof(AccessLog));
            _initialized = true;
        }
        finally
        {
            InitializationSemaphore.Release();
        }
    }

    public async Task LogAccessAsync(AccessLogType type, string route, string? remoteIp)
    {
        if (!_options.Value.Enabled) return;

        await InitializeAsync();
        if (_connection is null)
        {
            throw new InvalidOperationException("Can't access database");
        }
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