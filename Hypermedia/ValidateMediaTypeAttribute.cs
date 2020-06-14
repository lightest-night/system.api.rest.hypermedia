﻿using System.Linq;
using LightestNight.System.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

#pragma warning disable 1591

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public class ValidateMediaTypeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.ThrowIfNull(nameof(context));

            var mediaType = context.HttpContext.Request.Headers.ContainsKey(HeaderNames.Accept)
                ? context.HttpContext.Request.Headers[HeaderNames.Accept].FirstOrDefault()
                : Constants.JsonMediaType;

            if (!MediaTypeHeaderValue.TryParse(mediaType, out var acceptsHeader))
            {
                context.Result = new BadRequestObjectResult(
                    $"Media Type not present. Please add {HeaderNames.Accept} header with the required media type");
                return;
            }

            context.HttpContext.Items.Add(Constants.AcceptHeaderMediaType, acceptsHeader);
            base.OnActionExecuting(context);
        }
    }
}