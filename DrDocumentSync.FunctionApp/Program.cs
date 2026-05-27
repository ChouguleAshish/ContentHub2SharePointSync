using Azure.Identity;
using DrDocumentSync.FunctionApp.Infrastructure;
using DrDocumentSync.FunctionApp.Options;
using DrDocumentSync.FunctionApp.Persistence;
using DrDocumentSync.FunctionApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
              .AddEnvironmentVariables();

        var tempConfig = config.Build();
        var vaultUri = tempConfig["KeyVault:VaultUri"];
        if (!string.IsNullOrWhiteSpace(vaultUri))
        {
            config.AddAzureKeyVault(new Uri(vaultUri), new DefaultAzureCredential());
        }
    })
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.Configure<SyncOptions>(context.Configuration.GetSection(nameof(SyncOptions)));
        services.Configure<SitecoreOptions>(context.Configuration.GetSection(nameof(SitecoreOptions)));
        services.Configure<SharePointOptions>(context.Configuration.GetSection(nameof(SharePointOptions)));
        services.Configure<StateStoreOptions>(context.Configuration.GetSection(nameof(StateStoreOptions)));
        services.Configure<SitecoreSecretsOptions>(context.Configuration.GetSection("SitecoreSecrets"));

        services.AddHttpClient<ISitecoreApiClient, SitecoreApiClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<SitecoreOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        }).AddPolicyHandler(ResiliencePolicies.GetExponentialRetryPolicy());

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SharePointOptions>>().Value;
            var credential = new DefaultAzureCredential();
            return new GraphServiceClient(credential, ["https://graph.microsoft.com/.default"]);
        });

        services.AddSingleton<ISharePointService, SharePointService>();
        services.AddSingleton<IMetadataFilter, MetadataFilter>();
        services.AddSingleton<IMetadataTransformer, MetadataTransformer>();
        services.AddSingleton<ISyncStateStore, TableSyncStateStore>();
        services.AddSingleton<ISyncProcessor, SyncProcessor>();
    })
    .Build();

await host.RunAsync();
