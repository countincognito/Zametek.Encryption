using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using TestSupport.EfHelpers;
using Zametek.Shared.Encryption.EFCore.TestHelpers;
using Zametek.Utility.Cache;
using Zametek.Utility.Logging;

namespace Zametek.Access.Encryption.Tests
{
    public class EncryptionAccessFixture
        : IDisposable
    {
        public EncryptionAccessFixture()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            bool inMemory = config.GetValue<bool>("InMemory");

            //Configuration.Setup()
            //    .UseAzureTableStorage(config => config
            //        .ConnectionString("UseDevelopmentStorage=true"));

            ILogger serilog = new LoggerConfiguration().CreateLogger();

            Log.Logger = serilog;

            var serviceCollection = new ServiceCollection()
                .ActivateLogTypes(LogTypes.Tracking | LogTypes.Diagnostic | LogTypes.Error)
                .AddAutoMapper(typeof(EncryptionAccess))
                .TryAddSingletonWithLogProxy<ICacheUtility, CacheUtility>()
                .TryAddSingletonWithLogProxy<IEncryptionAccess, EncryptionAccess>()
                .AddSingleton(serilog)
                .Configure<CacheOptions>(config.GetSection("CacheOptions"))
                .AddDistributedMemoryCache();

            if (inMemory)
            {
                //serviceCollection.AddDbContextFactoryEx<EncryptionDbContext>(options => options.UseInMemoryDatabase("EncryptionDbConnection").ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning)));

                DbContextOptionsDisposable<EncryptionDbContext> options = SqliteInMemory.CreateOptions<EncryptionDbContext>(builder => builder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)));

                // https://github.com/JonPSmith/EfCore.TestSupport/blob/master/Version5UpgradeDocs.md
                options.TurnOffDispose();

                serviceCollection.AddDbContextFactoryEx<EncryptionDbContext>(() => options);
            }
            else
            {
                serviceCollection.AddDbContextFactoryEx<EncryptionDbContext>(options => options.UseSqlServer(config["EncryptionDbConnectionString"]));
            }

            ServerServices = serviceCollection.BuildServiceProvider();

            if (inMemory)
            {
                ServerServices.GetService<Func<EncryptionDbContext>>().Invoke().Database.EnsureCreated();
            }
            else
            {
                ServerServices.GetService<Func<EncryptionDbContext>>().Invoke().Database.Migrate();
            }
        }

        public IServiceProvider ServerServices { get; private set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
