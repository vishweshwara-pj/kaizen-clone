using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin()      // Allow any origin (you can replace this with specific origins if needed)
                        .AllowAnyMethod()      // Allow any HTTP method (GET, POST, etc.)
                        .AllowAnyHeader());    // Allow any header
});

// Register other services, such as logging, configuration, etc., if needed

var app = builder.Build();

// Use CORS
app.UseCors("AllowAllOrigins");

// Enable HTTPS redirection only in non-development environments
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();  // This will redirect HTTP to HTTPS in production
}

// Configure the middleware pipeline
app.UseRouting();
app.UseAuthorization();

// Map controllers (API routes)
app.MapControllers();

// Run the application
app.Run();
