namespace DrDocumentSync.FunctionApp.Options;

public sealed class SitecoreOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string AssetEndpoint { get; set; } = "/entities/query";
    public int PageSize { get; set; } = 200;
    public int TimeoutSeconds { get; set; } = 120;
    public List<string> ProductFilter { get; set; } = [];
    public List<string> ProductFamilyFilter { get; set; } = [];
    public List<string> ProductCategoryFilter { get; set; } = [];
}
