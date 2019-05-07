namespace GarbageCollectionApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Threading.Tasks;
    using GarbageCollectionApi.DataContracts;
    using GarbageCollectionApi.Examples;
    using GarbageCollectionApi.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Swashbuckle.AspNetCore.Filters;

    /// <summary>
    /// Controller for status
    /// </summary>
    [Route("api/status")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IStatusService statusService;

        public StatusController(IStatusService statusService)
        {
             this.statusService = statusService ?? throw new ArgumentNullException(nameof(statusService));
        }

        /// <summary>
        /// Gets data status
        /// </summary>
        /// <response code="404">If status does not exist</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DataRefreshStatus>> GetStatusAsync()
        {
            var status = await this.statusService.GetDataContractsStatusAsync().ConfigureAwait(false);

            if (status == null)
            {
                return this.NotFound();
            }

            return this.Ok(status);
        }
    }
}