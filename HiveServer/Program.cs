using ZLogger;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = builder.Configuration;

builder.WebHost.UseUrls("http://0.0.0.0:5241");

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IMemoryRepository, MemoryRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

SettingLogger();

builder.Services.AddControllers();

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
    if(fileDir == null)
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