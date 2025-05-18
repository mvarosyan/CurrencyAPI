using CurrencyAPI.Configuration;
using CurrencyAPI.Data;
using CurrencyAPI.Services;
using CurrencyAPI.Workers;
using CurrencyAPI.Handlers;
using CurrencyAPI.Cache;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddExceptionHandler<ExceptionHandler>();

builder.Services.AddHttpClient();

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<ICacheService, CacheService>();

builder.Services.AddScoped<ICustomCurrencyRepository, CustomCurrencyRepository>();

builder.Services.AddScoped<ICurrencyService, CurrencyService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<BackgroundWorkerService>();

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

// CORS must be before UseHttpsRedirection and other middleware
app.UseCors();

app.UseExceptionHandler(o => { });

// Comment out HTTPS redirection during development to avoid potential issues
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
