using Jcl.FileBrowser;
using Jcl.FileBrowser.AccessLogDatabase;
using Jcl.FileBrowser.Services.CodeViewer;
using Jcl.FileBrowser.Services.FileIcons;
using Jcl.FileBrowser.Services.MimeTypes;
using Jcl.FileBrowser.Services.RelativePath;
using Jcl.FileBrowser.Services.Thumbnails;
using Jcl.FileBrowser.Utils;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Jcl: add framework services
builder.Services
    .AddLogging(loggingBuilder =>
        loggingBuilder.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "hh:mm:ss ";
        }))
    .AddRazorPages();
builder.Services.AddHealthChecks();

// Jcl: add application services
builder.Services
    .AddMimeTypeMapper()
    .AddImageSharpMimeTypesAllowedManager()
    .AddFileIconManager()
    .AddCodeViewerManager()
    .AddRelativePathManager()
    .AddThumbnailBuilder()
    .AddAccessLogRepository();

// Jcl: configuration options
builder.Services
    .Configure<AccessLogOptions>(builder.Configuration.GetSection("AccessLogDb"))
    .Configure<GlobalOptions>(builder.Configuration.GetSection("Options"))
    .Configure<ForwardedHeadersOptions>(options => { options.ForwardedHeaders = ForwardedHeaders.All; });

await using var app = builder.Build();
ILogger? logger = null;

try
{
    logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Initialization");
    var globalOptions = app.Services.GetRequiredService<IOptions<GlobalOptions>>().Value;

    logger.LogInformation("Initializing {Title}", globalOptions.SiteTitle);
    if (args.Any(x => x == "--debug-config"))
    {
        logger.LogInformation("Configuration information: {Config}", ((IConfigurationRoot)app.Services.GetRequiredService<IConfiguration>()).GetDebugView());
    }

    ConfigurationChecker.AssertConfiguration(globalOptions);
    CacheETagUtils.Initialize(globalOptions);

    await using (var accessDbInitializer = app.Services.GetRequiredService<IAccessLogRepositoryInitialization>())
    {
        await accessDbInitializer.InitializeAsync();
    }

    app.UseForwardedHeaders();
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
    }

    app.UseHealthChecks("/healthcheck");
    app.UseStaticFiles();
    app.UseRouting();
    app.MapRazorPages();
}
catch (Exception e)
{
    if (logger is null)
    {
        await Console.Error.WriteLineAsync("Initialization exception: " + e);
    }
    else
    {
        logger.LogError(e, "Initialization exception: {Exception}", e.Message);
    }
    return -1;
}

try
{
    await app.RunAsync();
    return 0;
}
catch (Exception e)
{
    logger.LogError(e, "Initialization exception: {Exception}", e.Message);
    return -1;
}