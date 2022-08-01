namespace web_api.Infrastructure.Settings
{
    public interface IDatabaseSettings
    {
        string CollectionName { get; set; }
        string ConectionString { get; set; }
        string DatabaseName { get; set; }

    }
}
