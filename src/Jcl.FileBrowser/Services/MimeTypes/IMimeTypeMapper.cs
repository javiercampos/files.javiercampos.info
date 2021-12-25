namespace Jcl.FileBrowser.Services.MimeTypes;

public interface IMimeTypeMapper
{
    string Map(string fileName);
    string MapExtension(string extension);
}