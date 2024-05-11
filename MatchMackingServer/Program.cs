using ZLogger;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;
builder.Services.Configure<MatchingConfig>(configuration.GetSection(nameof(MatchingConfig)));

string? url = configuration.GetValue<string>("ServerUrl");
if(url == null)
{
    Console.WriteLine("ServerUrl is not set in appsettings.json");
    return;
}
builder.WebHost.UseUrls(url);

builder.Services.AddSingleton<IMatchWoker, MatchWoker>();

builder.Services.AddControllers();

SettingLogger();

var app = builder.Build();

ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
LogManager.SetLoggerFactory(loggerFactory, "Global");

app.MapControllers();

app.Run();

void SettingLogger()
{
    ILoggingBuilder logging = builder.Logging;
    IConfiguration config = builder.Configuration;
    _ = logging.ClearProviders();

    string? fileDir = config["LogDir"];
    if (fileDir == null)
        return;

    bool exists = Directory.Exists(fileDir);

    if (!exists)
    {
        _ = Directory.CreateDirectory(fileDir);
    }

    _ = logging.AddZLoggerRollingFile(
        options =>
        {
            options.UseJsonFormatter();
            options.FilePathSelector = (timestamp, sequenceNumber) => $"{fileDir}{timestamp.ToLocalTime():yyyy-MM-dd}_{sequenceNumber:000}.log";
            options.RollingInterval = ZLogger.Providers.RollingInterval.Day;
            options.RollingSizeKB = 1024;
        });

    _ = logging.AddZLoggerConsole(options =>
    {
        options.UseJsonFormatter();
    });
}