using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;

namespace Zametek.Shared.Encryption.EFCore.TestHelpers
{
    /// <summary>
    /// Copied from here:
    /// https://github.com/vany0114/EF.DbContextFactory/blob/master/src/Extensions/EFCore.DbContextFactory/Extensions/ServiceCollectionExtensions.cs
    /// With slight modifications.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="options">Returns the DbContext options.</param>
        public static IServiceCollection AddDbContextFactoryEx<TDataContext>(
            this IServiceCollection services,
            Func<DbContextOptions> options)
            where TDataContext : DbContext
            => AddDbContextFactoryEx<TDataContext>(
                services,
                (provider) => options.Invoke());

        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="optionsFunc">Service provider and DbContext options.</param>
        public static IServiceCollection AddDbContextFactoryEx<TDataContext>(
            this IServiceCollection services,
            Func<IServiceProvider, DbContextOptions> optionsFunc)
            where TDataContext : DbContext
        {
            AddCoreServicesEx<TDataContext>(services, optionsFunc, ServiceLifetime.Scoped);
            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<DbContextOptions<TDataContext>>();

            services.AddScoped<Func<TDataContext>>(ctx =>
            {
                return () => (TDataContext)Activator.CreateInstance(typeof(TDataContext), options);
            });

            return services;
        }

        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="options">Processes the DbContext options.</param>
        public static IServiceCollection AddDbContextFactoryEx<TDataContext>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> options,
            Func<DbContextOptionsBuilder<TDataContext>> dbContextBuilderFactory = null)
            where TDataContext : DbContext
            => AddDbContextFactoryEx(
                services,
                (provider, builder) => options.Invoke(builder),
                dbContextBuilderFactory);

        /// <summary>
        /// Configures the resolution of <typeparamref name="TDataContext"/>'s factory.
        /// </summary>
        /// <typeparam name="TDataContext">The DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="optionsAction">Service provider and DbContext options.</param>
        public static IServiceCollection AddDbContextFactoryEx<TDataContext>(
            this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            Func<DbContextOptionsBuilder<TDataContext>> dbContextBuilderFactory)
            where TDataContext : DbContext
        {
            AddCoreServicesEx(services, optionsAction, dbContextBuilderFactory, ServiceLifetime.Scoped);
            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<DbContextOptions<TDataContext>>();

            services.AddScoped<Func<TDataContext>>(ctx =>
            {
                return () => (TDataContext)Activator.CreateInstance(typeof(TDataContext), options);
            });

            return services;
        }

        private static void AddCoreServicesEx<TContextImplementation>(
            IServiceCollection services,
            Func<IServiceProvider, DbContextOptions> optionsFunc,
            ServiceLifetime optionsLifetime)
            where TContextImplementation : DbContext
        {
            services
                .AddMemoryCache()
                .AddLogging();

            services.TryAdd(
                new ServiceDescriptor(
                    typeof(DbContextOptions<TContextImplementation>),
                    p => optionsFunc(p),
                    optionsLifetime));

            services.Add(
                new ServiceDescriptor(
                    typeof(DbContextOptions),
                    p => p.GetRequiredService<DbContextOptions<TContextImplementation>>(),
                    optionsLifetime));
        }

        private static void AddCoreServicesEx<TContextImplementation>(
            IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            Func<DbContextOptionsBuilder<TContextImplementation>> dbContextBuilderFactory,
            ServiceLifetime optionsLifetime)
            where TContextImplementation : DbContext
        {
            AddCoreServicesEx<TContextImplementation>(
                services,
                p => DbContextOptionsFactoryEx(p, optionsAction, dbContextBuilderFactory),
                optionsLifetime);
        }

        private static DbContextOptions<TContext> DbContextOptionsFactoryEx<TContext>(
            IServiceProvider applicationServiceProvider,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            Func<DbContextOptionsBuilder<TContext>> dbContextBuilderFactory)
            where TContext : DbContext
        {
            DbContextOptionsBuilder<TContext> builder = dbContextBuilderFactory?.Invoke()
                ?? new DbContextOptionsBuilder<TContext>(
                    new DbContextOptions<TContext>(
                        new Dictionary<Type, IDbContextOptionsExtension>()));

            builder.UseApplicationServiceProvider(applicationServiceProvider);
            optionsAction?.Invoke(applicationServiceProvider, builder);

            return builder.Options;
        }
    }
}
