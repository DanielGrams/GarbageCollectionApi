namespace GarbageCollectionApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.NetworkInformation;
    using System.Threading.Tasks;
    using GarbageCollectionApi.DataContracts;
    using GarbageCollectionApi.Examples;
    using GarbageCollectionApi.Models;
    using GarbageCollectionApi.Services;
    using GarbageCollectionApi.Storage;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;

    /// <summary>
    /// Controller for status
    /// </summary>
    [Route("api/dump")]
    [ApiController]
    public class DumpController : ControllerBase
    {
        private readonly IDumpStorage storage;

        public DumpController(IDumpStorage storage)
        {
             this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        /// <summary>
        /// Gets zip compressed json dump file. The base element is of class DumpData.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(FileStreamResult), 200)]
        public async Task<IActionResult> GetFileAsync()
        {
            var stream = await this.storage.OpenReadAsync().ConfigureAwait(false);

            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = StorageSettings.BlobName,
                Inline = false,
            };

            this.Response.Headers["Content-Disposition"] = contentDisposition.ToString();

            return new FileStreamResult(stream, new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/zip"));
        }
    }
}