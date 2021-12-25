using SQLite;

namespace Jcl.FileBrowser.AccessLogDatabase;

[Table("Access")]
public class AccessLog
{
    public AccessLog(string accessLogType, string route, string? remoteIp)
    {
        AccessLogType = accessLogType;
        Route = route;
        RemoteIp = remoteIp;
    }

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Indexed, MaxLength(50)]
    public string AccessLogType { get; set; }
    [Indexed, MaxLength(500)]
    public string Route { get; set; }

    [Indexed, MaxLength(50)]
    public string? RemoteIp { get; set; }
    
}