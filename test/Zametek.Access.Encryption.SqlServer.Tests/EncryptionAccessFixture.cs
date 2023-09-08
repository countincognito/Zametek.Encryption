using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
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
                serviceCollection.AddPooledDbContextFactory<EncryptionDbContext>(optionsBuilder =>
                {
                    SqliteConnectionStringBuilder sqliteConnectionStringBuilder = new()
                    {
                        DataSource = @":memory:"
                    };
                    string connectionString = sqliteConnectionStringBuilder.ToString();
                    SqliteConnection sqliteConnection = new(connectionString);
                    sqliteConnection.Open();
                    optionsBuilder.UseSqlite(sqliteConnection);
                    optionsBuilder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                });
            }
            else
            {
                serviceCollection.AddPooledDbContextFactory<EncryptionDbContext>(optionsBuilder => optionsBuilder.UseSqlServer(config["EncryptionDbConnectionString"]));
            }

            ServerServices = serviceCollection.BuildServiceProvider();

            if (inMemory)
            {
                ServerServices.GetService<IDbContextFactory<EncryptionDbContext>>().CreateDbContext().Database.EnsureCreated();
            }
            else
            {
                ServerServices.GetService<IDbContextFactory<EncryptionDbContext>>().CreateDbContext().Database.Migrate();
            }
        }

        public IServiceProvider ServerServices { get; private set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
