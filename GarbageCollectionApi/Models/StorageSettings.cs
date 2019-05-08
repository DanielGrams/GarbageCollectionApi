namespace GarbageCollectionApi.Models
{
    public class StorageSettings
    {
        public const string ContainerName = "data";
        public const string BlobName = "dump.json.zip";

        public string ConnectionString { get; set; }
    }
}