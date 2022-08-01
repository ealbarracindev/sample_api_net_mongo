namespace web_api.Infrastructure.Settings
{
    public class DatabaseSettings:IDatabaseSettings
    {
        public string CollectionName { get; set; }
        public string ConectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
