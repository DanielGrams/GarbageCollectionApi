namespace GarbageCollectionApi.Storage
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IDumpStorage
    {
        Task<Stream> OpenWriteAsync();

        Task<Stream> OpenReadAsync();
    }
}