using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;

namespace Zametek.Access.Encryption
{
    public class EncryptionDbContext
        : DbContext
    {
        #region Fields

        public DbSet<SymmetricKey> SymmetricKeys { get; set; }

        #endregion

        #region Ctors

        public EncryptionDbContext(DbContextOptions<EncryptionDbContext> options)
            : base(options)
        {
        }

        #endregion

        #region Overrides

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            base.OnModelCreating(modelBuilder);

            // https://blog.dangl.me/archive/handling-datetimeoffset-in-sqlite-with-entity-framework-core/
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
                // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
                // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
                // use the DateTimeOffsetToBinaryConverter
                // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
                // This only supports millisecond precision, but should be sufficient for most use cases.
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
                                                                                || p.PropertyType == typeof(DateTimeOffset?));
                    foreach (var property in properties)
                    {
                        modelBuilder
                            .Entity(entityType.Name)
                            .Property(property.Name)
                            .HasConversion(new DateTimeOffsetToBinaryConverter());
                    }
                }
            }

            modelBuilder.Entity<SymmetricKey>()
              .HasKey(x => new
              {
                  x.SymmetricKeyId,
                  x.AsymmetricKeyId,
              });
            modelBuilder.Entity<SymmetricKey>()
                .Property(x => x.SymmetricKeyName)
                .IsRequired();
            modelBuilder.Entity<SymmetricKey>()
                .Property(x => x.AsymmetricKeyName)
                .IsRequired();
            modelBuilder.Entity<SymmetricKey>()
                .Property(x => x.AsymmetricKeyVersion)
                .IsRequired();
            modelBuilder.Entity<SymmetricKey>()
                .Property(x => x.WrappedSymmetricKey)
                .IsRequired();
            modelBuilder.Entity<SymmetricKey>()
                .Property(x => x.InitializationVector)
                .IsRequired();
            modelBuilder.Entity<SymmetricKey>()
                .Property(x => x.CreatedAt)
                .IsRequired();
            modelBuilder.Entity<SymmetricKey>()
                .Property(x => x.ModifiedAt)
                .IsRequired();
            modelBuilder.Entity<SymmetricKey>()
                .Property(x => x.IsDisabled)
                .IsRequired();
            modelBuilder.Entity<SymmetricKey>()
                .Property(x => x.IsDeleted)
                .IsRequired();
        }

        #endregion
    }
}
