using Polly;
using Polly.Extensions.Http;

namespace DrDocumentSync.FunctionApp.Infrastructure;

public static class ResiliencePolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetExponentialRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(r => (int)r.StatusCode == 429)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
