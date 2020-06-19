using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

#pragma warning disable 1591

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public static class ExtendsHttpContext
    {
        public static bool IsHateoas(this HttpContext httpContext)
        {
            var mediaType = (MediaTypeHeaderValue) httpContext.Items[Constants.AcceptHeaderMediaType];
            return mediaType.SubTypeWithoutSuffix.EndsWith(Constants.HateoasIdentifier,
                StringComparison.InvariantCultureIgnoreCase);
        }
    }
}