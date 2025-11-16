using ZEN.Controller;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.ApplyDInjectionService(builder.Configuration);

var app = builder.Build();

// Configure middleware and endpoints
await app.ApplyWebBuilder();

app.MapGet("/healthcheck", () => "Server is alive!");

app.Run();
