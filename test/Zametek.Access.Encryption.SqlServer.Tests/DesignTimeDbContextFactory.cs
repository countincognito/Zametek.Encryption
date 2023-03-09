using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Zametek.Access.Encryption.Tests
{
    public class DesignTimeDbContextFactory
        : IDesignTimeDbContextFactory<EncryptionDbContext>
    {
        public EncryptionDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<EncryptionDbContext>();
            builder.UseSqlServer(
                @"Server = (LocalDb)\MSSQLLocalDB; Database = Encryption; Trusted_Connection = True; MultipleActiveResultSets = true;",
               options => options.MigrationsAssembly("Zametek.Access.Encryption.SqlServer.Tests"));
            return new EncryptionDbContext(builder.Options);
        }
    }
}
