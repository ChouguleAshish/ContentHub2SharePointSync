using DrDocumentSync.FunctionApp.Models;
using DrDocumentSync.FunctionApp.Options;
using Microsoft.Extensions.Options;

namespace DrDocumentSync.FunctionApp.Services;

public sealed class MetadataFilter(IOptions<SitecoreOptions> options) : IMetadataFilter
{
    public bool IsMatch(DocumentAsset asset)
    {
        return InFilter(asset.Product, options.Value.ProductFilter)
            && InFilter(asset.ProductFamily, options.Value.ProductFamilyFilter)
            && InFilter(asset.ProductCategory, options.Value.ProductCategoryFilter);
    }

    private static bool InFilter(string value, IReadOnlyCollection<string> filters)
        => filters.Count == 0 || filters.Contains(value, StringComparer.OrdinalIgnoreCase);
}
