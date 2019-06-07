namespace GarbageCollectionApi.Models
{
    public class AuthorizationSettings
    {
        #pragma warning disable CA1819 // Properties should not return arrays
        public string[] Keys { get; set; }
        #pragma warning restore CA1819
    }
}