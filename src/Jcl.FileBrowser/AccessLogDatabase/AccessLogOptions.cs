namespace Jcl.FileBrowser.AccessLogDatabase;

public class AccessLogOptions
{
    public bool Enabled { get; set; } = true;
    public string DbPath { get; set; } = "access.db";
}