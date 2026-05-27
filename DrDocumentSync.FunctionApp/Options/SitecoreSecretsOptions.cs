namespace DrDocumentSync.FunctionApp.Options;

public sealed class SitecoreSecretsOptions
{
    public string ClientIdSecretName { get; set; } = string.Empty;
    public string ClientSecretSecretName { get; set; } = string.Empty;
}
