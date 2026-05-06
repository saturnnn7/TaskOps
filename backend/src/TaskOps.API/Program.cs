using TaskOps.API.Extensions;
using TaskOps.API.Options;
using TaskOps.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── Options ──────────────────────────────────────────────────────────────────
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

builder.Services.Configure<SwaggerOptions>(
    builder.Configuration.GetSection(SwaggerOptions.SectionName));

// ── Controllers ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Authentication & Authorization ───────────────────────────────────────────
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

// ── Cache ─────────────────────────────────────────────────────────────────────
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// ── Swagger ───────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

// ──────────────────────────────────────────────────────────────────────────────

var app = builder.Build();

// ── Swagger middleware (dev + if enabled) ─────────────────────────────────────
var swaggerOptions = app.Configuration
    .GetSection(SwaggerOptions.SectionName)
    .Get<SwaggerOptions>();

if (swaggerOptions?.Enabled == true)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskOps API v1");
        options.RoutePrefix = "swagger";
    });
}

// ── Middleware pipeline ───────────────────────────────────────────────────────
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();