using DrDocumentSync.FunctionApp.Models;
using DrDocumentSync.FunctionApp.Options;
using DrDocumentSync.FunctionApp.Services;
using Microsoft.Extensions.Options;

namespace DrDocumentSync.Tests;

public sealed class MetadataFilterTests
{
    [Fact]
    public void IsMatch_ReturnsTrue_WhenAllFiltersMatch()
    {
        var options = Options.Create(new SitecoreOptions
        {
            ProductFilter = ["A"], ProductFamilyFilter = ["B"], ProductCategoryFilter = ["C"]
        });
        var filter = new MetadataFilter(options);
        var asset = new DocumentAsset { Product = "A", ProductFamily = "B", ProductCategory = "C" };

        Assert.True(filter.IsMatch(asset));
    }
}
