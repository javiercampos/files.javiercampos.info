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
    .Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.All;
    })
    .Configure<RouteOptions>(options =>
    {
        options.ConstraintMap.Add(BrowsePrefixConstraint.PrefixName, typeof(BrowsePrefixConstraint));
    });

var app = builder.Build();

try
{
    var globalOptions = app.Services.GetRequiredService<IOptions<GlobalOptions>>().Value;

    Console.WriteLine("Initializing " + globalOptions.SiteTitle);
    if (args.Any(x => x == "--debug-config"))
    {
        Console.WriteLine(((IConfigurationRoot)app.Services.GetRequiredService<IConfiguration>()).GetDebugView());    
    }
    ConfigurationChecker.AssertConfiguration(globalOptions);
    CacheETagUtils.Initialize(globalOptions);

    app.UseForwardedHeaders();
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
    }

    app.UseStaticFiles();
    app.UseRouting();
    app.MapRazorPages();
}
catch (Exception e)
{
    Console.Error.WriteLine("Initialization exception: " + e);
    return -1;
}

try
{
    app.Run();
    return 0;
}
catch (Exception e)
{
    Console.Error.WriteLine("Runtime exception: " + e);
    return -1;
}
