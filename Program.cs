using Microsoft.Extensions.Options;
using mypaperwork.AppSettings;
using Serilog;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddSingleton<AppSettings>(appSettings => appSettings.GetRequiredService<IOptions<AppSettings>>().Value);

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin 
    .AllowCredentials());
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
