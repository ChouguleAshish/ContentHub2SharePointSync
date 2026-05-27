using DrDocumentSync.FunctionApp.Models;

namespace DrDocumentSync.FunctionApp.Services;

public sealed class MetadataTransformer : IMetadataTransformer
{
    public SharePointDocument Transform(DocumentAsset source) => new()
    {
        FileName = source.FileName.Trim(),
        Title = source.Title.Trim(),
        Description = source.Description.Trim(),
        Product = source.Product.Trim(),
        ProductFamily = source.ProductFamily.Trim(),
        ProductCategory = source.ProductCategory.Trim(),
        LastModifiedUtc = source.LastModifiedUtc,
        FileHash = source.FileHash?.Trim().ToLowerInvariant()
    };
}
