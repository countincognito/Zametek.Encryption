using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using Zametek.Utility;
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

            string dbType = config.GetValue<string>("DbType");

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

            dbType.ValueSwitchOn()
                .Case(@"Memory", _ =>
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

                    ServerServices = serviceCollection.BuildServiceProvider();
                    using var ctx = ServerServices.GetService<IDbContextFactory<EncryptionDbContext>>().CreateDbContext();
                    ctx.Database.EnsureCreated();
                })
                .Case(@"SqlServer", _ =>
                {
                    serviceCollection.AddPooledDbContextFactory<EncryptionDbContext>(
                        options => options.UseSqlServer(
                            config["EncryptionDbSqlServerConnectionString"],
                            optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(SqlServerDesignTimeDbContextFactory).Assembly.FullName)));
                    ServerServices = serviceCollection.BuildServiceProvider();

                    using var ctx = ServerServices.GetService<IDbContextFactory<EncryptionDbContext>>().CreateDbContext();
                    ctx.Database.Migrate();
                })
                .Case(@"Npgsql", _ =>
                {
                    serviceCollection.AddPooledDbContextFactory<EncryptionDbContext>(
                        options => options.UseNpgsql(
                            config["EncryptionDbNpgsqlConnectionString"],
                            optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(NpgsqlDesignTimeDbContextFactory).Assembly.FullName)));
                    ServerServices = serviceCollection.BuildServiceProvider();

                    using var ctx = ServerServices.GetService<IDbContextFactory<EncryptionDbContext>>().CreateDbContext();
                    ctx.Database.Migrate();
                })
                .Default(x => throw new InvalidOperationException($@"Unrecognized database type {x}"));
        }

        public IServiceProvider ServerServices { get; private set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
