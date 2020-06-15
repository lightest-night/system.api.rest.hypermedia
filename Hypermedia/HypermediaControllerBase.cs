using System;
using System.Collections.Generic;
using System.Linq;
using LightestNight.System.Api.Rest.Hypermedia.Data;
using LightestNight.System.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

#pragma warning disable 1591

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public class HypermediaControllerBase : ControllerBase
    {
        #nullable enable
        private LinkGenerator? _linkGenerator;
        #nullable restore

        private LinkGenerator LinkGenerator
        {
            get
            {
                if (_linkGenerator != null)
                    return _linkGenerator;
    
                var linkGenerator = HttpContext?.RequestServices?.GetRequiredService<LinkGenerator>();
                _linkGenerator = linkGenerator.ThrowIfNull(nameof(linkGenerator));
    
                return _linkGenerator!;
            }
    
            set => _linkGenerator = value.ThrowIfNull(nameof(value));
        }
    
        [NonAction]
        public virtual CreatedResult Created(object value)
        {
            var entityLinkDefs = LinkDefinitions.GetEntityLinkDefinitions(GetType(), value).ToArray();
            var rootForResource = entityLinkDefs.SingleOrDefault(link => link.IsRootForResource);
            if (rootForResource == null)
                throw new InvalidOperationException("No link has been defined as root for this resource");
            
            var location = new Uri(LinkGenerator.GetUriByAction(HttpContext, rootForResource.Action, values: rootForResource.Value).ToLowerInvariant());

            var mediaType = (MediaTypeHeaderValue) HttpContext.Items[Constants.AcceptHeaderMediaType];
            if (!mediaType.SubTypeWithoutSuffix.EndsWith(Constants.HateoasIdentifier,
                StringComparison.InvariantCultureIgnoreCase))
                return base.Created(location, value);

            var links = entityLinkDefs.Select(linkDef =>
                new Link(LinkGenerator.GetUriByAction(HttpContext, linkDef.Action, values: linkDef.Value),
                    linkDef.Relation, linkDef.Method));

            var dataShaper = new DataShaper();
            var shapedEntity = dataShaper.ShapeData(value).Entity;
            shapedEntity.Add(Constants.LinksKey, links);

            return base.Created(location, shapedEntity);
        }

        public override OkObjectResult Ok(object value)
        {
            var mediaType = (MediaTypeHeaderValue) HttpContext.Items[Constants.AcceptHeaderMediaType];
            if (!mediaType.SubTypeWithoutSuffix.EndsWith(Constants.HateoasIdentifier,
                StringComparison.InvariantCultureIgnoreCase))
                return base.Ok(value);

            var entityLinkDefs = LinkDefinitions.GetEntityLinkDefinitions(GetType(), value);
            var links = entityLinkDefs.Select(linkDef =>
                new Link(LinkGenerator.GetUriByAction(HttpContext, linkDef.Action, values: linkDef.Value),
                    linkDef.Relation, linkDef.Method));

            var dataShaper = new DataShaper();
            var shapedEntity = dataShaper.ShapeData(value).Entity;
            shapedEntity.Add(Constants.LinksKey, links);

            return base.Ok(shapedEntity);
        }

        [NonAction]
        public OkObjectResult Ok(IEnumerable<object> value)
        {
            var controllerType = GetType();

            var valueArray = value as object[] ?? value.ToArray();
            var mediaType = (MediaTypeHeaderValue) HttpContext.Items[Constants.AcceptHeaderMediaType];
            if (!mediaType.SubTypeWithoutSuffix.EndsWith(Constants.HateoasIdentifier,
                StringComparison.InvariantCultureIgnoreCase))
                return base.Ok(value);

            var dataShaper = new DataShaper();
            var shapedValues = dataShaper.ShapeData(valueArray).ToArray();
            var shapedEntities = shapedValues.Select(entity => entity.Entity).ToList();
            for (var index = 0; index < shapedValues.Length; index++)
            {
                var originalValue = shapedValues[index].Original;
                if (originalValue == null)
                    continue;

                var entityLinkDefs = LinkDefinitions.GetEntityLinkDefinitions(controllerType, originalValue);
                var links = entityLinkDefs.Select(linkDef =>
                    new Link(LinkGenerator.GetUriByAction(HttpContext, linkDef.Action, values: linkDef.Value),
                        linkDef.Relation, linkDef.Method));

                shapedEntities[index].Add(Constants.LinksKey, links);
            }

            var wrapper = new LinkCollection<Entity>(shapedEntities);
            var resourceLinkDefs = LinkDefinitions.GetResourceLinkDefinitions(controllerType).ToArray();
            wrapper.Links.AddRange(resourceLinkDefs.Select(linkDef =>
                new Link(LinkGenerator.GetUriByAction(HttpContext, linkDef.Action, values: linkDef.Value),
                    linkDef.Relation, linkDef.Method)));

            return base.Ok(wrapper);
        }
    }
}