using System;
using System.Collections.Generic;
using System.Linq;
using LightestNight.System.Api.Rest.Hypermedia.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public class HypermediaControllerBase : ControllerBase
    {
        private readonly DataShaper _dataShaper = new();
        private LinkGenerator? _linkGenerator;

        private LinkGenerator LinkGenerator
        {
            get
            {
                if (_linkGenerator != null)
                    return _linkGenerator;

                var linkGenerator = HttpContext?.RequestServices.GetRequiredService<LinkGenerator>();
                _linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));

                return _linkGenerator!;
            }
        }

        [NonAction]
        public virtual CreatedResult Created(object value)
        {
            var controllerType = GetType();
            var modelType = value.GetType();
            var entityLinkDefs = LinkDefinitions.GetEntityLinkDefinitions(controllerType, modelType).ToArray();
            var rootForResource = entityLinkDefs.SingleOrDefault(link => link.IsRootForResource);
            if (rootForResource == null)
                throw new InvalidOperationException("No link has been defined as root for this resource");

            var rootValues = rootForResource.GetValueMap(value);
            var location = new Uri(LinkGenerator.GetUriByAction(HttpContext, rootForResource.Action, values: rootValues).ToLowerInvariant());

            if (!HttpContext.IsHateoas())
                return base.Created(location, value);

            var links = entityLinkDefs.Select(linkDef =>
            {
                var linkValues = linkDef.GetValueMap(value);
                return new Link(LinkGenerator.GetUriByAction(HttpContext, linkDef.Action, values: linkValues),
                    linkDef.Relation, linkDef.Method);
            });

            var shapedEntity = _dataShaper.ShapeData(value).Entity;
            shapedEntity.Add(Constants.LinksKey, links);

            return base.Created(location, shapedEntity);
        }

        public override OkObjectResult Ok(object value)
        {
            if (!HttpContext.IsHateoas())
                return base.Ok(value);

            var entityLinkDefs = LinkDefinitions.GetEntityLinkDefinitions(GetType(), value.GetType());
            var links = entityLinkDefs.Select(linkDef =>
            {
                var linkValues = linkDef.GetValueMap(value);
                return new Link(LinkGenerator.GetUriByAction(HttpContext, linkDef.Action, values: linkValues),
                    linkDef.Relation, linkDef.Method);
            });

            var shapedEntity = _dataShaper.ShapeData(value).Entity;
            shapedEntity.Add(Constants.LinksKey, links);

            return base.Ok(shapedEntity);
        }

        [NonAction]
        public OkObjectResult Ok(IEnumerable<object> value)
        {
            if (!HttpContext.IsHateoas())
                return base.Ok(value);

            var valueArray = value as object[] ?? value.ToArray();
            var shapedValues = _dataShaper.ShapeData(valueArray).ToArray();
            var shapedEntities = shapedValues.Select(entity => entity.Entity).ToArray();
            var controllerType = GetType();
            var modelType = valueArray[0].GetType();

            for (var index = 0; index < shapedValues.Length; index++)
            {
                var shapedValue = shapedValues[index];
                var entityLinkDefs = LinkDefinitions.GetEntityLinkDefinitions(controllerType, modelType);
                var links = entityLinkDefs.Select(linkDef =>
                {
                    var linkValues = linkDef.GetValueMap(shapedValue);
                    return new Link(LinkGenerator.GetUriByAction(HttpContext, linkDef.Action, values: linkValues),
                        linkDef.Relation, linkDef.Method);
                });

                shapedEntities[index].Add(Constants.LinksKey, links);
            }

            var wrapper = new LinkCollection<Entity>(shapedEntities);
            var resourceLinkDefs = LinkDefinitions.GetResourceLinkDefinitions(controllerType, modelType).ToArray();
            wrapper.Links.AddRange(resourceLinkDefs.Select(linkDef =>
                new Link(LinkGenerator.GetUriByAction(HttpContext, linkDef.Action), linkDef.Relation, linkDef.Method)));

            return base.Ok(wrapper);
        }
    }
}