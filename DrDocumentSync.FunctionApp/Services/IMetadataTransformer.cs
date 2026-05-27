using DrDocumentSync.FunctionApp.Models;

namespace DrDocumentSync.FunctionApp.Services;

public interface IMetadataTransformer
{
    SharePointDocument Transform(DocumentAsset source);
}
