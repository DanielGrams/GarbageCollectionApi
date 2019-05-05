namespace GarbageCollectionApi.Utils
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> logger;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Catch generic exception
            catch (Exception ex)
#pragma warning restore CA1031
            {
                this.logger.LogError($"Exception in middleware: {ex}");
                await HandleExceptionAsync(context).ConfigureAwait(false);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error",
            }.ToString());
        }
    }
}