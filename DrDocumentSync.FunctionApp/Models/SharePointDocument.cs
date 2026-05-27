namespace DrDocumentSync.FunctionApp.Models;

public sealed class SharePointDocument
{
    public string FileName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string ProductFamily { get; set; } = string.Empty;
    public string ProductCategory { get; set; } = string.Empty;
    public DateTimeOffset LastModifiedUtc { get; set; }
    public string? FileHash { get; set; }
}
