namespace GarbageCollectionApi.Storage
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Extensions.Options;

    public class AzureDumpStorage : IDumpStorage
    {
        private readonly IOptions<StorageSettings> settings;
        private CloudStorageAccount storageAccount;
        private CloudBlobContainer cloudBlobContainer;
        private CloudBlockBlob cloudBlockBlob;

        public AzureDumpStorage(IOptions<StorageSettings> settings)
        {
            this.settings = settings ?? throw new System.ArgumentNullException(nameof(settings));
        }

        public async Task<Stream> OpenReadAsync()
        {
            await this.Init().ConfigureAwait(false);
            return await this.cloudBlockBlob.OpenReadAsync().ConfigureAwait(false);
        }

        public async Task<Stream> OpenWriteAsync()
        {
            await this.Init().ConfigureAwait(false);
            return await this.cloudBlockBlob.OpenWriteAsync().ConfigureAwait(false);
        }

        private async Task Init()
        {
            if (this.storageAccount != null &&
                this.cloudBlobContainer != null &&
                this.cloudBlockBlob != null)
            {
                return;
            }

            if (CloudStorageAccount.TryParse(this.settings.Value?.ConnectionString, out var storageAccount))
            {
                this.storageAccount = storageAccount;
            }
            else
            {
                throw new InvalidOperationException("Invalid azure storage connection string");
            }

            var cloudBlobClient = this.storageAccount.CreateCloudBlobClient();
            this.cloudBlobContainer = cloudBlobClient.GetContainerReference(StorageSettings.ContainerName);
            await this.cloudBlobContainer.CreateIfNotExistsAsync().ConfigureAwait(false);

            this.cloudBlockBlob = this.cloudBlobContainer.GetBlockBlobReference(StorageSettings.BlobName);
        }
    }
}