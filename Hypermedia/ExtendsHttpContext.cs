using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public static class ExtendsHttpContext
    {
        public static bool IsHateoas(this HttpContext httpContext)
        {
            if (httpContext.Items[Constants.AcceptHeaderMediaType] is not MediaTypeHeaderValue mediaType)
                return false;

            return mediaType.SubTypeWithoutSuffix.EndsWith(Constants.HateoasIdentifier,
                StringComparison.InvariantCultureIgnoreCase);
        }
    }
}