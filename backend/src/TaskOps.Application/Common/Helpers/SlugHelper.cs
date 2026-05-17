using System.Text.RegularExpressions;

namespace TaskOps.Application.Common.Helpers;

/// <summary>
/// Generates URL-friendly slugs from project names.
/// Example: "My New Project!" → "my-new-project"
/// </summary>
public static partial class SlugHelper
{
    [GeneratedRegex(@"[^a-z0-9\s-]")]
    private static partial Regex NonAlphanumericRegex();

    [GeneratedRegex(@"[\s-]+")]
    private static partial Regex MultipleSpacesRegex();

    public static string Generate(string name)
    {
        var slug = name.ToLowerInvariant().Trim();

        // Remove all non-alphanumeric characters except spaces and hyphens
        slug = NonAlphanumericRegex().Replace(slug, string.Empty);

        // Replace spaces and multiple hyphens with single hyphen
        slug = MultipleSpacesRegex().Replace(slug, "-");

        return slug.Trim('-');
    }

    /// <summary>
    /// Generates a unique slug by appending a short random suffix.
    /// Used when the base slug is already taken.
    /// </summary>
    public static string GenerateUnique(string name)
        => $"{Generate(name)}-{Guid.NewGuid().ToString()[..8]}";
}