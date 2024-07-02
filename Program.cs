using Microsoft.Extensions.Options;
using mypaperwork;
using mypaperwork.Utils;
using Serilog;
using mypaperwork.Middlewares;
using mypaperwork.Services.Logging;
using mypaperwork.Services.User;
using Microsoft.Extensions.DependencyInjection.Extensions;
using mypaperwork.Services.Testing;
using mypaperwork.Models.Database;
using mypaperwork.Services.Document;
using SQLite;

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
// SQLite-net
var appSettings = builder.Services.BuildServiceProvider().GetService<AppSettings>(); // Tip: retrieve added AppSettings service above for using
var sqliteDB = new SQLiteAsyncConnection(Path.Combine(Directory.GetCurrentDirectory(), appSettings.SQLiteDBPath));
await sqliteDB.CreateTableAsync<Logs>();
await sqliteDB.CreateTableAsync<Users>();
await sqliteDB.CreateTableAsync<Files>();
await sqliteDB.CreateTableAsync<UsersFiles>();
await sqliteDB.CreateTableAsync<Categories>();
await sqliteDB.CreateTableAsync<PaperWorks>();
await sqliteDB.CreateTableAsync<PaperWorksCategories>();
await sqliteDB.CreateTableAsync<Documents>();
builder.Services.AddSingleton<SQLiteAsyncConnection>(sqliteDB);
builder.Services.AddTransient<JWTUtils>();
builder.Services.AddTransient<UserServices>();
builder.Services.AddTransient<LoggingServices>();
builder.Services.AddTransient<TestingServices>();
builder.Services.AddTransient<DocumentServices>();

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
