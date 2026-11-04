namespace TechMove.Core.Services;

/// <summary>
/// Validates file uploads for contracts (e.g., PDF agreements).
/// </summary>
public static class UploadValidator
{
    private static readonly HashSet<string> ValidMimeTypes = new()
    {
        "application/pdf"
    };

    private static readonly HashSet<string> ValidExtensions = new()
    {
        ".pdf"
    };

    /// <summary>
    /// Validates if a file is a valid PDF.
    /// </summary>
    public static bool IsValidPdf(string fileName, string contentType)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLower();
        return ValidExtensions.Contains(extension) && ValidMimeTypes.Contains(contentType);
    }
}
