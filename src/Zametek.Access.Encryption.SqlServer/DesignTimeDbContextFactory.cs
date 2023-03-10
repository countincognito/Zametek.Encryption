using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Zametek.Access.Encryption;

namespace Zametek.Access.Encryption
{
    public class DesignTimeDbContextFactory
        : IDesignTimeDbContextFactory<EncryptionDbContext>
    {
        public EncryptionDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<EncryptionDbContext>();
            builder.UseSqlServer(
                @"Server = (LocalDb)\MSSQLLocalDB; Database = Encryption; Trusted_Connection = True; MultipleActiveResultSets = true;");
            return new EncryptionDbContext(builder.Options);
        }
    }
}
