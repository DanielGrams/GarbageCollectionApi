namespace GarbageCollectionApi.Utils
{
    using System;
    using System.Linq;
    using GarbageCollectionApi.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizationKeyFilterAttribute : Attribute, IAuthorizationFilter
    {
        public const string ApiKeyHeaderName = "X-API-KEY";
        private readonly IOptions<AuthorizationSettings> settings;

        public AuthorizationKeyFilterAttribute(IOptions<AuthorizationSettings> settings)
            : base()
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowedKeys = this.settings.Value.Keys;

            if (allowedKeys == null || !allowedKeys.Any())
            {
                return;
            }

            var headerKey = context.HttpContext.Request.Headers[ApiKeyHeaderName].FirstOrDefault();

            if (headerKey != null && allowedKeys.Any(allowedKey => headerKey == allowedKey))
            {
                return;
            }

            context.Result = new UnauthorizedResult();
        }
    }
}