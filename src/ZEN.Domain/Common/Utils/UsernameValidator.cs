using System.Text.RegularExpressions;

namespace ZEN.Domain.Common.Utils;

public static class UsernameValidator
{
    private static readonly string[] ReservedWords = {
        "admin", "api", "www", "public", "private", "register", "login", 
        "logout", "profile", "a-dmin"
    };

    private static readonly Regex UsernameRegex = new Regex(
        @"^[a-z0-9_-]{3,30}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public static bool IsValid(string? username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        // Check length
        if (username.Length < 3 || username.Length > 30)
            return false;

        // Check format (only a-z, 0-9, -, _)
        if (!UsernameRegex.IsMatch(username))
            return false;

        // Check cannot start or end with - or _
        if (username.StartsWith("-") || username.StartsWith("_") ||
            username.EndsWith("-") || username.EndsWith("_"))
            return false;

        // Check reserved words (case-insensitive)
        if (ReservedWords.Contains(username.ToLowerInvariant()))
            return false;

        return true;
    }

    public static string Normalize(string username)
    {
        return username.ToLowerInvariant().Trim();
    }

    public static string GenerateSlug(string username)
    {
        return Normalize(username)
            .Replace(" ", "-")
            .Replace("_", "-");
    }
}

