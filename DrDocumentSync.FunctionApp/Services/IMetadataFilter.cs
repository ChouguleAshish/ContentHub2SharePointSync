using DrDocumentSync.FunctionApp.Models;

namespace DrDocumentSync.FunctionApp.Services;

public interface IMetadataFilter
{
    bool IsMatch(DocumentAsset asset);
}
