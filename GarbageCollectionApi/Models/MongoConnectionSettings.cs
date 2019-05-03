namespace GarbageCollectionApi.Models
{
    public class MongoConnectionSettings
    {
        public const string TownsCollectionName = "Towns";
        public const string EventsCollectionName = "Events";

        public string ConnectionString { get; set; }

        public string Database { get; set; }
    }
}