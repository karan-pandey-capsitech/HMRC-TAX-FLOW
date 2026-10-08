namespace HMRC_TAX_FLOW.Infrastructure.MongoDB;

public sealed class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}