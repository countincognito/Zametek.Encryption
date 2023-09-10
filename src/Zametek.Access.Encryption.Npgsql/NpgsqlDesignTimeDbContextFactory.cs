using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Zametek.Access.Encryption
{
    public class NpgsqlDesignTimeDbContextFactory
        : IDesignTimeDbContextFactory<EncryptionDbContext>
    {
        public EncryptionDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<EncryptionDbContext>();
            builder.UseNpgsql(
                @"Server = localhost; Database = Encryption; Port = 5432; User ID = postgres; Password = postgres; Integrated Security = true; Pooling = true;",
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(NpgsqlDesignTimeDbContextFactory).Assembly.FullName));
            return new EncryptionDbContext(builder.Options);
        }
    }
}
