namespace TaskOps.API.Options;

/// <summary>
/// Controls Swagger/Scalar UI availability per environment.
/// </summary>
public sealed class SwaggerOptions
{
    public const string SectionName = "Swagger";

    public bool Enabled { get; init; } = false;
}