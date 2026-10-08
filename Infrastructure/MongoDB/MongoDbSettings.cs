namespace HMRC_TAX_FLOW.Infrastructure.MongoDB
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = default!;
        public string DatabaseName { get; set; } = default!;
    }
}