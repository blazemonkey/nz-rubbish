using NZRubbishCollection.Api;
using NZRubbishCollection.Shared.Services.ScrapingService;

#if(DEBUG)
// uncomment this for testing
// args = new[] { "--types", "3", "--council", "Auckland City Council", "--street", "" };
#endif
// parse the command line arguments
CommandLine.Parse(args);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddScoped<IScrapingService, ScrapingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
