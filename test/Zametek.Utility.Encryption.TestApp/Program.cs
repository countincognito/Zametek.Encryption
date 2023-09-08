using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Threading.Tasks;
using Xunit;
using Zametek.Access.Encryption;
using Zametek.Utility;
using Zametek.Utility.Cache;
using Zametek.Utility.Encryption;
using Zametek.Utility.Logging;

// Adding JSON file into IConfiguration.
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

ILogger serilog = new LoggerConfiguration()
    .Enrich.FromLogProxy()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

bool inMemory = config.GetValue<bool>("InMemory");
bool contextMethods = config.GetValue<bool>("ContextMethods");

Log.Logger = serilog;

var serviceCollection = new ServiceCollection()
    .ActivateLogTypes(LogTypes.Tracking | LogTypes.Diagnostic | LogTypes.Error)
    .AddAutoMapper(typeof(EncryptionAccess))
    .AddSingleton<ICacheUtility, CacheUtility>()
    .AddSingleton<IEncryptionUtility, EncryptionUtility>()
    .AddSingleton<ISymmetricKeyEncryption, AesEncryption>()
    .AddSingleton<IEncryptionAccess, EncryptionAccess>()
    .AddSingleton(serilog)
    .Configure<CacheOptions>(config.GetSection("CacheOptions"))
    .AddDistributedMemoryCache();

if (inMemory)
{
    serviceCollection.AddSingleton<IAsymmetricKeyVault>(new FakeKeyVault());

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
    serviceCollection.AddSingleton<IAsymmetricKeyVault, AzureKeyVault>()
        .Configure<AzureKeyVaultOptions>(options => config.Bind("AzureKeyVaultOptions", options));

    serviceCollection.AddPooledDbContextFactory<EncryptionDbContext>(optionsBuilder => optionsBuilder.UseSqlServer(config["EncryptionDbConnectionString"]));
}

var serviceProvider = serviceCollection.BuildServiceProvider();

if (inMemory)
{
    serviceProvider.GetService<Func<EncryptionDbContext>>().Invoke().Database.EnsureCreated();
}
else
{
    serviceProvider.GetService<Func<EncryptionDbContext>>().Invoke().Database.Migrate();
}

IEncryptionUtility encryptionUtility = serviceProvider.GetService<IEncryptionUtility>();

if (contextMethods)
{
    await ContextMethodsAsync(encryptionUtility);
}
else
{
    await RegularMethodsAsync(encryptionUtility);
}

static async Task ContextMethodsAsync(IEncryptionUtility encryptionUtility)
{
    ArgumentNullException.ThrowIfNull(encryptionUtility);

    (SymmetricKeyDefinition symmetricKeyDefinition, AsymmetricKeyDefinition asymmetricKeyDefinition) =
        await encryptionUtility.CreateSymmetricKeyIdAsync(Guid.NewGuid().ToDashedString(), default);

    EncryptionContext.NewCurrent(symmetricKeyDefinition.Id);

    Console.WriteLine();

    Console.WriteLine(@"*********************************");
    Console.WriteLine(@$"* New Symmetric key ID: {symmetricKeyDefinition.Id}");
    Console.WriteLine(@$"* New Symmetric key Name: {symmetricKeyDefinition.Name}");
    Console.WriteLine(@$"* New Asymmetric key ID: {asymmetricKeyDefinition.Id}");
    Console.WriteLine(@$"* New Asymmetric key Name: {asymmetricKeyDefinition.Name}");
    Console.WriteLine(@$"* New Asymmetric key Version: {asymmetricKeyDefinition.Version}");
    Console.WriteLine(@"*********************************");

    Console.WriteLine();

    Console.WriteLine(@"Please enter some text to encrypt:");

    string randomValue = Console.ReadLine();

    byte[] encryptedData = await encryptionUtility.EncryptObjectAsync(randomValue, default);

    Console.WriteLine();

    Console.WriteLine(@"*********************************");
    Console.WriteLine(@$"* Encrypted data: {encryptedData.ByteArrayToBase64String()}");
    Console.WriteLine(@"*********************************");

    Console.WriteLine();

    Console.WriteLine(@"Press any key to continue...");
    Console.ReadLine();

    string decryptedData = await encryptionUtility.DecryptObjectAsync<string>(encryptedData, default);

    Assert.Equal(randomValue, decryptedData);

    Console.WriteLine();

    Console.WriteLine(@"*********************************");
    Console.WriteLine(@$"* Decrypted data: {decryptedData}");
    Console.WriteLine(@"*********************************");

    Console.WriteLine();

    Console.WriteLine(@"Press any key to continue...");
    Console.ReadLine();

    Console.WriteLine(@"*********************************");
    Console.WriteLine(@$"* Now rotate Asymmetric key");
    Console.WriteLine(@"*********************************");

    Console.WriteLine();

    Console.WriteLine(@"Press any key to continue...");
    Console.ReadLine();

    (SymmetricKeyDefinition symmetricKeyDefinition2, AsymmetricKeyDefinition asymmetricKeyDefinition2) =
        await encryptionUtility.RotateAsymmetricKeyAsync(default);

    Console.WriteLine(@"*********************************");
    Console.WriteLine(@$"* New Symmetric key ID: {symmetricKeyDefinition2.Id}");
    Console.WriteLine(@$"* New Symmetric key Name: {symmetricKeyDefinition2.Name}");
    Console.WriteLine(@$"* New Asymmetric key ID: {asymmetricKeyDefinition2.Id}");
    Console.WriteLine(@$"* New Asymmetric key Name: {asymmetricKeyDefinition2.Name}");
    Console.WriteLine(@$"* New Asymmetric key Version: {asymmetricKeyDefinition2.Version}");
    Console.WriteLine(@"*********************************");
}

static async Task RegularMethodsAsync(IEncryptionUtility encryptionUtility)
{
    ArgumentNullException.ThrowIfNull(encryptionUtility);

    var createKeysRequest = new CreateKeysRequest
    {
        SymmetricKeyName = Guid.NewGuid().ToDashedString(),
        AsymmetricKeyName = Guid.NewGuid().ToDashedString(),
    };

    CreateKeysResponse createKeyResponse = await encryptionUtility.CreateKeysAsync(createKeysRequest, default);

    Console.WriteLine();

    Console.WriteLine(@"*********************************");
    Console.WriteLine(@$"* New Symmetric key ID: {createKeyResponse.SymmetricKeyDefinition.Id}");
    Console.WriteLine(@$"* New Symmetric key Name: {createKeyResponse.SymmetricKeyDefinition.Name}");
    Console.WriteLine(@$"* New Asymmetric key ID: {createKeyResponse.AsymmetricKeyDefinition.Id}");
    Console.WriteLine(@$"* New Asymmetric key Name: {createKeyResponse.AsymmetricKeyDefinition.Name}");
    Console.WriteLine(@$"* New Asymmetric key Version: {createKeyResponse.AsymmetricKeyDefinition.Version}");
    Console.WriteLine(@"*********************************");

    Console.WriteLine();

    var viewAsymmetricKeyDefinitionRequest = new ViewAsymmetricKeyDefinitionRequest
    {
        Name = createKeyResponse.AsymmetricKeyDefinition.Name,
        Version = createKeyResponse.AsymmetricKeyDefinition.Version,
    };

    ViewAsymmetricKeyDefinitionResponse viewAsymmetricKeyDefinitionResponse =
        await encryptionUtility.ViewAsymmetricKeyDefinitionAsync(viewAsymmetricKeyDefinitionRequest, default);

    Console.WriteLine();

    Console.WriteLine(@"*********************************");
    Console.WriteLine(@$"* New Asymmetric key ID: {viewAsymmetricKeyDefinitionResponse.AsymmetricKeyDefinition.Id}");
    Console.WriteLine(@$"* New Asymmetric key Name: {viewAsymmetricKeyDefinitionResponse.AsymmetricKeyDefinition.Name}");
    Console.WriteLine(@$"* New Asymmetric key Version: {viewAsymmetricKeyDefinitionResponse.AsymmetricKeyDefinition.Version}");
    Console.WriteLine(@"*********************************");

    Console.WriteLine();

    Console.WriteLine(@"Please enter some text to encrypt:");

    string randomValue = Console.ReadLine();

    var encryptRequest = new EncryptRequest
    {
        SymmetricKeyId = createKeyResponse.SymmetricKeyDefinition.Id,
        Data = randomValue.StringToByteArray(),
    };

    EncryptResponse encryptResponse = await encryptionUtility.EncryptAsync(encryptRequest, default);

    Console.WriteLine();

    Console.WriteLine(@"*********************************");
    Console.WriteLine(@$"* Encrypted data: {encryptResponse.EncryptedData.ByteArrayToBase64String()}");
    Console.WriteLine(@"*********************************");

    Console.WriteLine();

    Console.WriteLine(@"Press any key to continue...");
    Console.ReadLine();

    var decryptRequest = new DecryptRequest
    {
        SymmetricKeyId = createKeyResponse.SymmetricKeyDefinition.Id,
        EncryptedData = encryptResponse.EncryptedData,
    };

    DecryptResponse decryptResponse = await encryptionUtility.DecryptAsync(decryptRequest, default);

    string output = decryptResponse.Data.ByteArrayToString();

    Assert.Equal(randomValue, output);

    Console.WriteLine();

    Console.WriteLine(@"*********************************");
    Console.WriteLine(@$"* Decrypted data: {output}");
    Console.WriteLine(@"*********************************");

    Console.WriteLine();

    Console.WriteLine(@"Press any key to continue...");
    Console.ReadLine();

    Console.WriteLine(@"*********************************");
    Console.WriteLine(@$"* Now rotate Asymmetric key");
    Console.WriteLine(@"*********************************");

    Console.WriteLine();

    Console.WriteLine(@"Press any key to continue...");
    Console.ReadLine();

    var rotateAsymmetricKeyRequest = new RotateAsymmetricKeyRequest
    {
        SymmetricKeyId = createKeyResponse.SymmetricKeyDefinition.Id,
    };

    RotateAsymmetricKeyResponse rotateAsymmetricKeyResponse = await encryptionUtility
        .RotateAsymmetricKeyAsync(rotateAsymmetricKeyRequest, default);

    Console.WriteLine();

    var viewAsymmetricKeyDefinitionRequest2 = new ViewAsymmetricKeyDefinitionRequest
    {
        Name = rotateAsymmetricKeyResponse.AsymmetricKeyDefinition.Name,
        Version = rotateAsymmetricKeyResponse.AsymmetricKeyDefinition.Version,
    };

    ViewAsymmetricKeyDefinitionResponse viewAsymmetricKeyDefinitionResponse2 =
        await encryptionUtility.ViewAsymmetricKeyDefinitionAsync(viewAsymmetricKeyDefinitionRequest2, default);

    Console.WriteLine();

    Console.WriteLine(@"*********************************");
    Console.WriteLine(@$"* Rotated Asymmetric key ID: {viewAsymmetricKeyDefinitionResponse2.AsymmetricKeyDefinition.Id}");
    Console.WriteLine(@$"* Rotated Asymmetric key Name: {viewAsymmetricKeyDefinitionResponse2.AsymmetricKeyDefinition.Name}");
    Console.WriteLine(@$"* Rotated Asymmetric key Version: {viewAsymmetricKeyDefinitionResponse2.AsymmetricKeyDefinition.Version}");
    Console.WriteLine(@"*********************************");
}
