using ZLogger;
using APIServer;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;
builder.Services.Configure<SocketConfig>(Configuration.GetSection("SocketConfig"));

string? url = Configuration.GetValue<string>("ServerUrl");
if(url == null)
{
    Console.WriteLine("ServerUrl is not set in appsettings.json");
    return;
}
builder.WebHost.UseUrls(url);

// 서비스 등록
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IMatchService, MatchService>();

SettingLogger();

// 레포지토리 등록
builder.Services.AddSingleton<IMemoryRepository, MemoryRepository>();
builder.Services.AddSingleton<RedisLockRepository>();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAttendanceCheckRepository, AttendanceCheckRepository>();
builder.Services.AddScoped<IMailRepository, MailRepository>();

builder.Services.AddControllers();

var app = builder.Build();

ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
LogManager.SetLoggerFactory(loggerFactory, "Global");

app.UseMiddleware<TokenCheckMiddleware>();

app.UseMiddleware<RequestOneCheckMiddleware>();

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