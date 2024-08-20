using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using mypaperwork;
using mypaperwork.Utils;
using Serilog;
using mypaperwork.Middlewares;
using mypaperwork.Services.Logging;
using mypaperwork.Services.User;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using mypaperwork.Services.Testing;
using mypaperwork.Models.Database;
using mypaperwork.Services.Category;
using mypaperwork.Services.Document;
using mypaperwork.Services.FileServices;
using mypaperwork.Services.Paperwork;
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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mypaperwork API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}_swagger_doc.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.Configure<AppSettings>(builder.Configuration);
// Configure forwarded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownProxies.Add(IPAddress.Parse("192.168.1.17"));
});
builder.Services.AddSingleton<AppSettings>(appSettings => appSettings.GetRequiredService<IOptions<AppSettings>>().Value);
// SQLite-net
var appSettings = builder.Services.BuildServiceProvider().GetService<AppSettings>(); // Tip: retrieve added AppSettings service above for using
var sqliteDB = new SQLiteAsyncConnection(Path.Combine(Directory.GetCurrentDirectory(), appSettings.SQLiteDBPath));
await sqliteDB.CreateTableAsync<Logs>();
await sqliteDB.CreateTableAsync<Users>();
await sqliteDB.CreateTableAsync<FilesDBModel>();
await sqliteDB.CreateTableAsync<UsersFiles>();
await sqliteDB.CreateTableAsync<Categories>();
await sqliteDB.CreateTableAsync<Paperworks>();
await sqliteDB.CreateTableAsync<PaperworksCategories>();
await sqliteDB.CreateTableAsync<Documents>();
await sqliteDB.CreateTableAsync<Settings>();
await sqliteDB.CreateTableAsync<UsersSettings>();
builder.Services.AddSingleton<SQLiteAsyncConnection>(sqliteDB);
builder.Services.AddTransient<JWTUtils>();
builder.Services.AddTransient<DBUtils>();
builder.Services.AddTransient<UserServices>();
builder.Services.AddTransient<LoggingServices>();
builder.Services.AddTransient<CategoryServices>();
builder.Services.AddTransient<PaperworkServices>();
builder.Services.AddTransient<FileServices>();
builder.Services.AddTransient<TestingServices>();
builder.Services.AddTransient<DocumentServices>();
builder.Services.AddTransient<HttpContextUtils>();

var app = builder.Build();
app.UseMiddleware<EnableRequestRewindMiddleware>();
app.UseMiddleware<JwtMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
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
app.MapGet("/", () => "Hello ForwardedHeadersOptions!");
app.Run();
