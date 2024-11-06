using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using RockPaperScissorsGame.Infrastructure;
using RockPaperScissorsGame.Services;
using RockPaperScissorsGame.Configuration;
using RockPaperScissorsGame.Validators;

var builder = WebApplication.CreateBuilder(args);

// Bind the application to port 5081
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5081);
});

// Configure services
builder.Services.Configure<RandomNumberSettings>(builder.Configuration.GetSection("RandomNumberSettings"));
builder.Services.AddControllers();
builder.Services.AddHttpClient<RandomNumberService>();
builder.Services.AddScoped<IGameService, GameService>();

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new QueryStringApiVersionReader("api-version")
    );
});

// Configure versioned API documentation in Swagger
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Configure Swagger and Swagger options
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// Register handlers and validators
builder.Services.AddScoped<PlayGameCommandHandler>();
builder.Services.AddScoped<GetChoicesQueryHandler>();
builder.Services.AddScoped<GetRandomChoiceQueryHandler>();
builder.Services.AddScoped<PlayGameCommandValidator>();
builder.Services.AddScoped<GetChoicesQueryValidator>();
builder.Services.AddScoped<GetRandomChoiceQueryValidator>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure Swagger to be available in all environments
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"RockPaperScissors API {description.GroupName.ToUpperInvariant()}");
    }
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowAllOrigins");

app.MapControllers();
app.Run();

public partial class Program { }
