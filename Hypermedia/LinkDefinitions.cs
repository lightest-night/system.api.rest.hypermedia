using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

#pragma warning disable 1591

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public static class LinkDefinitions
    {
        private static readonly IDictionary<Type, ReadOnlyDictionary<Type, ReadOnlyCollection<LinkDefinition>>>
            EntityLinkDefinitions =
                GetLinkDefinitions(nameof(ControllerMap<object, object>.EntityLinkDefinitions));
        
        private static readonly IDictionary<Type, ReadOnlyDictionary<Type, ReadOnlyCollection<LinkDefinition>>>
            ResourceLinkDefinitions =
                GetLinkDefinitions(nameof(ControllerMap<object, object>.ResourceLinkDefinitions));

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
                        mapType.BaseType.GetGenericTypeDefinition() == typeof(ControllerMap<,>))
                    .Select(Activator.CreateInstance)
                    .ToArray();

                foreach (var map in nullableMaps)
                {
                    if (map == null)
                        continue;

                    yield return map;
                }
            }
        }

        public static IEnumerable<LinkDefinition> GetEntityLinkDefinitions<TController, TModel>()
            => GetEntityLinkDefinitions(typeof(TController), typeof(TModel));

        public static IEnumerable<LinkDefinition> GetEntityLinkDefinitions(Type controllerType, Type modelType)
        {
            var linkDefs = EntityLinkDefinitions[controllerType][modelType];
            return GetLinkDefinitions(linkDefs);
        }

        public static IEnumerable<LinkDefinition> GetResourceLinkDefinitions<TController, TModel>()
            => GetResourceLinkDefinitions(typeof(TController), typeof(TModel));
        
        public static IEnumerable<LinkDefinition> GetResourceLinkDefinitions(Type controllerType, Type modelType)
        {
            var linkDefs = ResourceLinkDefinitions[controllerType][modelType];
            return GetLinkDefinitions(linkDefs);
        }

        private static IDictionary<Type, ReadOnlyDictionary<Type, ReadOnlyCollection<LinkDefinition>>> GetLinkDefinitions(string propertyName)
        {
            var typedMap = Maps.ToDictionary(map => map.GetType().BaseType?.GenericTypeArguments[0]);
            var result = new Dictionary<Type, ReadOnlyDictionary<Type, ReadOnlyCollection<LinkDefinition>>>();

            foreach (var (key, value) in typedMap)
            {
                if (key == null)
                    continue;
                
                var mapType = value.GetType();     // Instance of ControllerMap<,>
                if (!(mapType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)?.GetValue(value) is
                    IDictionary modelTypedLinkDefinitions))
                    continue;

                if (!(modelTypedLinkDefinitions.Values is IEnumerable linkDefinitions))
                    continue;

                foreach (var linkDefinition in linkDefinitions.Cast<ReadOnlyDictionary<Type, ReadOnlyCollection<LinkDefinition>>>()
                    .Where(linkDefinition => linkDefinition != null))
                    result.Add(key, linkDefinition);
            }

            return result;
        }

        private static IEnumerable<LinkDefinition> GetLinkDefinitions(IEnumerable<LinkDefinition> linkDefs)
        {
            var result = new List<LinkDefinition>(linkDefs.Where(linkDef => linkDef != null));
            var rootForResource = result.FirstOrDefault(link => link.IsRootForResource) ?? result.FirstOrDefault(link =>
                string.Equals(link.Action, Constants.DefaultRootForResourceAction,
                    StringComparison.InvariantCultureIgnoreCase));
            if (rootForResource != null && !rootForResource.IsRootForResource)
                rootForResource.IsRootForResource = true;

            return result;
        }
    }
}