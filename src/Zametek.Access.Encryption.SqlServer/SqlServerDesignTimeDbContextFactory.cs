using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Zametek.Access.Encryption
{
    public class SqlServerDesignTimeDbContextFactory
        : IDesignTimeDbContextFactory<EncryptionDbContext>
    {
        public EncryptionDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<EncryptionDbContext>();
            builder.UseSqlServer(
                @"Server = (LocalDb)\MSSQLLocalDB; Database = Encryption; Trusted_Connection = True; MultipleActiveResultSets = true;",
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(SqlServerDesignTimeDbContextFactory).Assembly.FullName));
            return new EncryptionDbContext(builder.Options);
        }
    }
}
