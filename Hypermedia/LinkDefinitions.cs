using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#pragma warning disable 1591

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public static class LinkDefinitions
    {
        private const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        private static readonly IDictionary<Type, ICollection> EntityLinkDefinitions =
            GetLinkDefinitions(nameof(ControllerMap<object>.EntityLinkDefinitions));
        
        private static readonly IDictionary<Type, ICollection> ResourceLinkDefinitions =
            GetLinkDefinitions(nameof(ControllerMap<object>.ResourceLinkDefinitions));

        private static IEnumerable<object> Maps
        {
            get
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var nullableMaps = assemblies.Where(assembly => !assembly.IsDynamic)
                    .SelectMany(assembly => assembly.GetExportedTypes())
                    .Where(mapType =>
                        mapType.IsClass && !mapType.IsAbstract && mapType.BaseType != null &&
                        mapType.BaseType.IsGenericType &&
                        mapType.BaseType.GetGenericTypeDefinition() == typeof(ControllerMap<>))
                    .Select(Activator.CreateInstance)
                    .ToArray();

                var maps = new List<object>();
                foreach (var map in nullableMaps)
                {
                    if (map == null)
                        continue;

                    maps.Add(map);
                }

                return maps;
            }
        }

        public static IEnumerable<LinkDefinition> GetEntityLinkDefinitions(Type controllerType)
        {
            var linkDefs = EntityLinkDefinitions[controllerType];
            return GetLinkDefinitions(linkDefs);
        }

        public static IEnumerable<LinkDefinition> GetResourceLinkDefinitions(Type controllerType)
        {
            var linkDefs = ResourceLinkDefinitions[controllerType];
            return GetLinkDefinitions(linkDefs);
        }

        private static IEnumerable<LinkDefinition> GetLinkDefinitions(ICollection linkDefs)
        {
            var linkDefType = typeof(LinkDefinition);
            var result = new List<LinkDefinition>();

            foreach (var linkDefCollection in linkDefs)
            {
                if (!(linkDefCollection is IEnumerable linkDefEnumerable))
                    continue;

                result.AddRange(linkDefEnumerable.Cast<object?>().Where(linkDef => linkDef != null && linkDef.GetType() == linkDefType).Cast<LinkDefinition>());
            }

            var rootForResource = result.FirstOrDefault(link => link.IsRootForResource) ?? result.FirstOrDefault(link =>
                string.Equals(link.Action, Constants.DefaultRootForResourceAction,
                    StringComparison.InvariantCultureIgnoreCase));
            if (rootForResource != null && !rootForResource.IsRootForResource)
                rootForResource.IsRootForResource = true;

            return result;
        }
        
        private static Dictionary<Type, ICollection> GetLinkDefinitions(string propertyName)
        {
            var controllerTypes = Maps.ToDictionary(map => map.GetType().BaseType?.GenericTypeArguments[0]);

            var result = new Dictionary<Type, ICollection>();
            foreach (var (key, value) in controllerTypes)
            {
                if (key == null)
                    continue;

                var mapType = value.GetType();
                if (!(mapType.GetProperty(propertyName, PropertyBindingFlags)?.GetValue(value) is IDictionary
                    linkDefinitions))
                    continue;

                result.Add(key, linkDefinitions.Values);
            }

            return result;
        }
    }
}