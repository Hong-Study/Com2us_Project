var builder = WebApplication.CreateBuilder(args);

// 서비스 등록
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IAttendanceCheckService, AttendanceCheckService>();

// 레포지토리 등록
builder.Services.AddSingleton<IMemoryRepository, MemoryRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAttendanceCheckRepository, AttendanceCheckRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseMiddleware<TokenCheckMiddleware>();

app.MapControllers();

app.Run();