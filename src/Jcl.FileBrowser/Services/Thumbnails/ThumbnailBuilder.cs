using Jcl.FileBrowser.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace Jcl.FileBrowser.Services.Thumbnails;

public class ThumbnailBuilder : IThumbnailBuilder
{
    public async Task<MemoryStream?> BuildThumbnail(int width, int height, string path)
    {
        Image image;
        await using (var imageStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            var imageFormat = await Image.DetectFormatAsync(imageStream);
            if (imageFormat is null)
            {
                return null;
            }

            imageStream.Seek(0, SeekOrigin.Begin);
            image = await Image.LoadAsync<Rgba32>(imageStream);
        }

        MemoryStream outputStream;
        using (image)
        {
            image.Mutate(process =>
            {
                process.Resize(new ResizeOptions
                    {
                        Size = new Size(width, height), Sampler = new BicubicResampler(), Mode = ResizeMode.Pad,
                        PremultiplyAlpha = false
                    })
                    .BackgroundColor(new Rgba32(255, 255, 255, 0));
            });
            outputStream = new MemoryStream();
            await image.SaveAsPngAsync(outputStream);
        }

        outputStream.Seek(0, SeekOrigin.Begin);
        return outputStream;
    }
    
    public string BuildThumbnailCacheTag(DateTime lastModified, int width, int height)
    {
        return MiscUtils.CombineHashesLong(lastModified, width, height).ToString("x");
    }
    
    
}