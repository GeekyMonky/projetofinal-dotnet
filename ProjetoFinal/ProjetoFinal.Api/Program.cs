using ProjetoFinal.BusinessContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure form options for file uploads
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB limit
    options.MemoryBufferThreshold = int.MaxValue;
});

// Configure Kestrel server limits for file uploads
builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
});

// Add CORS if your Blazor app is on a different port
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("https://localhost:7212", "http://localhost:5000") // Adjust ports as needed
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<IBusinessContext, BusinessContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Use CORS before authorization
app.UseCors("AllowBlazorApp");

app.UseAuthorization();

// Configure static files to serve from WebApp's wwwroot directory
var webAppWwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ProjetoFinal.WebApp", "wwwroot");
if (Directory.Exists(webAppWwwRootPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(webAppWwwRootPath),
        RequestPath = ""
    });
}

app.MapControllers();

app.Run();
