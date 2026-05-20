using FluentValidation;
using TaskOps.API.Extensions;
using TaskOps.API.Middleware;
using TaskOps.API.Options;
using TaskOps.Application.Common.Interfaces;
using TaskOps.Application.Services;
using TaskOps.Application.Validators;
using TaskOps.Infrastructure;
using TaskOps.Application.Common.Options;
using TaskOps.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Options ───────────────────────────────────────────────────────────────────
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<SwaggerOptions>(
    builder.Configuration.GetSection(SwaggerOptions.SectionName));

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Infrastructure (EF Core, Redis, Auth) ────────────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);

// ── Application services ──────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ICommentService, CommentService>();

// ── Validation ────────────────────────────────────────────────────────────────
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

// ── Authentication & Authorization ───────────────────────────────────────────
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

// ── Cache ─────────────────────────────────────────────────────────────────────
builder.Services.AddRedisCache(builder.Configuration);

// ── Swagger ───────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Global middleware ─────────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<ValidationMiddleware>();

// ── Swagger UI ────────────────────────────────────────────────────────────────
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

// ── Pipeline ──────────────────────────────────────────────────────────────────
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();