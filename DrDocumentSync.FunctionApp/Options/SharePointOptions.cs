namespace DrDocumentSync.FunctionApp.Options;

public sealed class SharePointOptions
{
    public string TenantId { get; set; } = string.Empty;
    public string SiteId { get; set; } = string.Empty;
    public string DriveId { get; set; } = string.Empty;
    public string RootFolder { get; set; } = "DR-Service-Documents";
}
