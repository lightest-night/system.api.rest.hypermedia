using System.Linq;
using LightestNight.System.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public class RootController : ControllerBase
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public RootController(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        [HttpGet("routes", Name = "Get Hypermedia Routes")]
        public IActionResult GetRoutes()
        {
            var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items
                .Where(ad => ad.AttributeRouteInfo != null).Select(ad => new
                {
                    Name = ad.AttributeRouteInfo.Name,
                    Template = ad.AttributeRouteInfo.Template
                }).ToArray();

            return Ok(routes);
        }
    }
}