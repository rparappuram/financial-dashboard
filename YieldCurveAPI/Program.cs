using YieldCurveAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<TreasuryXmlService>();
builder.Services.AddScoped<RateCalculatorService>();
builder.Services.AddScoped<YieldDataService>();
builder.Services.AddScoped<ChartGenerationService>();
builder.Services.AddScoped<FileGenerationService>();

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Replace with your frontend's URL
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowReactApp");

app.MapControllers();
app.Run();