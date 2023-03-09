using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using Zametek.Utility.Logging;

namespace Zametek.Utility.Encryption.Tests
{
    public class AzureKeyVaultFixture
        : IDisposable
    {
        public AzureKeyVaultFixture()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            bool inMemory = config.GetValue<bool>("InMemory");

            ILogger serilog = new LoggerConfiguration().CreateLogger();

            Log.Logger = serilog;

            var serviceCollection = new ServiceCollection()
                .ActivateLogTypes(LogTypes.Tracking | LogTypes.Diagnostic | LogTypes.Error)
                .Configure<AzureKeyVaultOptions>(options => config.Bind("AzureKeyVaultOptions", options))
                .AddSingleton(serilog);

            if (inMemory)
            {
                serviceCollection.TryAddSingletonWithLogProxy<IAsymmetricKeyVault, FakeKeyVault>();
            }
            else
            {
                serviceCollection.TryAddSingletonWithLogProxy<IAsymmetricKeyVault, AzureKeyVault>();
            }

            ServerServices = serviceCollection.BuildServiceProvider();
        }

        public IServiceProvider ServerServices { get; private set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
