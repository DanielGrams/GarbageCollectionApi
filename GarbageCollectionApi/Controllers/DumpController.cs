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
    using GarbageCollectionApi.Services;
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
        private readonly IDumpService dumpService;

        public DumpController(IDumpService dumpService)
        {
             this.dumpService = dumpService ?? throw new ArgumentNullException(nameof(dumpService));
        }

        /// <summary>
        /// Gets zip compressed json dump file. The base element is of class DumpData.
        /// </summary>
        /// <response code="404">If file does not exist</response>
        [HttpGet]
        [ProducesResponseType(typeof(FileStreamResult), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetFile()
        {
            string filePath = this.dumpService.ZipFilePath;

            if (!System.IO.File.Exists(filePath))
            {
                return this.NotFound();
            }

            var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            return new FileStreamResult(stream, new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/zip"))
            {
                FileDownloadName = "dump.zip",
            };
        }
    }
}