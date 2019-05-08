namespace GarbageCollectionApi.Storage
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Extensions.Options;

    public class LocalDumpStorage : IDumpStorage
    {
        private readonly string basePath;
        private readonly string filePath;

        public LocalDumpStorage()
        {
            this.basePath = Path.Combine(Path.GetTempPath(), "de.goslargarbagecollectionapi");
            this.filePath = Path.Combine(this.basePath, StorageSettings.BlobName);
        }

        public async Task<Stream> OpenReadAsync()
        {
            return await Task.Run(() => new FileStream(this.filePath, FileMode.OpenOrCreate)).ConfigureAwait(false);
        }

        public async Task<Stream> OpenWriteAsync()
        {
            return await Task.Run(() =>
             {
                 if (!Directory.Exists(this.basePath))
                 {
                     Directory.CreateDirectory(this.basePath);
                 }

                 if (File.Exists(this.filePath))
                 {
                     File.Delete(this.filePath);
                 }

                 return new FileStream(this.filePath, FileMode.Create);
             }).ConfigureAwait(false);
        }
    }
}