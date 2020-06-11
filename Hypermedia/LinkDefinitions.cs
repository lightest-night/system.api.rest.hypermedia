using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public static class LinkDefinitions
    {
        private const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        private static readonly IDictionary<Type, ICollection> EntityLinkDefinitions =
            GetLinkDefinitions(nameof(ControllerMap<object, object>.EntityLinkDefinitions));

        private static readonly IDictionary<Type, ICollection> ResourceLinkDefinitions =
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
        
        public static IEnumerable<LinkDefinition> GetEntityLinkDefinitions(Type controllerType, object value)
        {
            const BindingFlags invocationBindingFlags = BindingFlags.InvokeMethod | PropertyBindingFlags;

            var valueType = value.GetType();
            var linkDefs = EntityLinkDefinitions[controllerType];
            var linkDefType = typeof(LinkDefinition<>).MakeGenericType(valueType);
            var result = new List<LinkDefinition>();

            foreach (var linkDefCollection in linkDefs)
            {
                if (!(linkDefCollection is IEnumerable linkDefEnumerable))
                    continue;

                foreach (var linkDef in linkDefEnumerable)
                {
                    if (linkDef == null || linkDef.GetType() != linkDefType)
                        continue;

                    var action = linkDefType.GetProperty(nameof(LinkDefinition<object>.Action), PropertyBindingFlags)
                        ?.GetValue(linkDef);
                    var relation = linkDefType
                        .GetProperty(nameof(LinkDefinition<object>.Relation), PropertyBindingFlags)?.GetValue(linkDef);
                    var method = linkDefType.GetProperty(nameof(LinkDefinition<object>.Method), PropertyBindingFlags)
                        ?.GetValue(linkDef);
                    var valueExpression = linkDefType
                        .GetProperty(nameof(LinkDefinition<object>.ValueExpression), PropertyBindingFlags)
                        ?.GetValue(linkDef);
                    var isRootForResource = linkDefType
                        .GetProperty(nameof(LinkDefinition<object>.IsRootForResource), PropertyBindingFlags)
                        ?.GetValue(linkDef);

                    var funcType = typeof(Func<,>).MakeGenericType(valueType, typeof(object));
                    var expressionType = typeof(Expression<>).MakeGenericType(funcType);
                    var func = expressionType.InvokeMember(nameof(Expression<object>.Compile), invocationBindingFlags,
                        null, valueExpression, null, CultureInfo.InvariantCulture);

                    var valueExpressionResult = funcType.InvokeMember(nameof(Func<object>.Invoke),
                        invocationBindingFlags, null, func, new[] {value}, CultureInfo.InvariantCulture);

                    result.Add(new LinkDefinition(action?.ToString()!, relation?.ToString()!, (HttpMethod) method!,
                        valueExpressionResult!, (bool) isRootForResource!));
                }
            }

            var rootForResource = result.FirstOrDefault(link => link.IsRootForResource)
                                  ?? result.FirstOrDefault(link => string.Equals(link.Action,
                                      Constants.DefaultRootForResourceAction,
                                      StringComparison.InvariantCultureIgnoreCase));
            if (rootForResource != null && !rootForResource.IsRootForResource)
                rootForResource.IsRootForResource = true;

            return result;
        }

        public static IEnumerable<LinkDefinition> GetEntityLinkDefinitions(Type controllerType, IEnumerable<object> values)
        {
            var valueArray = values as object[] ?? values.ToArray();
            return valueArray.SelectMany(entity => GetEntityLinkDefinitions(controllerType, entity));
        }

        public static IEnumerable<LinkDefinition> GetResourceLinkDefinitions(Type controllerType)
        {
            var linkDefs = ResourceLinkDefinitions[controllerType];
            var linkDefType = typeof(LinkDefinition);

            foreach (var linkDefCollection in linkDefs)
            {
                if (!(linkDefCollection is IEnumerable linkDefEnumerable))
                    continue;

                foreach (var linkDef in linkDefEnumerable)
                {
                    if (linkDef == null || linkDef.GetType() != linkDefType)
                        continue;

                    var action = linkDefType.GetProperty(nameof(LinkDefinition.Action), PropertyBindingFlags)
                        ?.GetValue(linkDef);
                    var relation = linkDefType.GetProperty(nameof(LinkDefinition.Relation), PropertyBindingFlags)
                        ?.GetValue(linkDef);
                    var method = linkDefType.GetProperty(nameof(LinkDefinition.Method), PropertyBindingFlags)
                        ?.GetValue(linkDef);
                    var value = linkDefType.GetProperty(nameof(LinkDefinition.Value), PropertyBindingFlags)
                        ?.GetValue(linkDef);

                    yield return new LinkDefinition(action?.ToString()!, relation?.ToString()!, (HttpMethod) method!,
                        value!);
                }
            }
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
                var readModelType = mapType.BaseType?.GenericTypeArguments[1];
                if (readModelType == null)
                    continue;

                if (!(mapType.GetProperty(propertyName, PropertyBindingFlags)?.GetValue(value) is IDictionary
                    linkDefinitions))
                    continue;

                result.Add(key, linkDefinitions.Values);
            }

            return result;
        }
    }
}