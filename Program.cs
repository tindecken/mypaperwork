using Microsoft.Extensions.Options;
using mypaperwork;
using mypaperwork.Utils;
using Serilog;
using mypaperwork.Middlewares;
using mypaperwork.Services.Logging;
using mypaperwork.Services.TestingServices;
using mypaperwork.Services.User;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Logging.ClearProviders();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("Logs/Mypaperwork_.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 62)
    .CreateLogger();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddSingleton<AppSettings>(appSettings => appSettings.GetRequiredService<IOptions<AppSettings>>().Value);
builder.Services.AddTransient<JWTUtils>();
builder.Services.AddTransient<UserServices>();
builder.Services.AddTransient<LoggingServices>();
builder.Services.AddTransient<TestingServices>();

var app = builder.Build();
app.UseMiddleware<EnableRequestRewindMiddleware>();
app.UseMiddleware<JwtMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin 
    .AllowCredentials());

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
