using YieldCurveAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<TreasuryXmlService>();
builder.Services.AddScoped<RateCalculatorService>();
builder.Services.AddScoped<YieldDataService>();
builder.Services.AddScoped<ChartGenerationService>();
builder.Services.AddScoped<FileGenerationService>();

// Add CORS Policy to Allow All Origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin() // Allows requests from any origin
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

// Enable CORS with the "AllowAll" policy
app.UseCors("AllowAll");

app.MapControllers();
app.Run();