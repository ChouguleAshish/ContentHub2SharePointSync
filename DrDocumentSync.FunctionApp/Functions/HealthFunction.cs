using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace DrDocumentSync.FunctionApp.Functions;

public sealed class HealthFunction
{
    [Function("Health")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "health")] HttpRequestData req)
    {
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.WriteString("OK");
        return response;
    }
}
