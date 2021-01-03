using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;

#pragma warning disable 1591

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public abstract class ControllerMap<TController, TModel>
    {
        private readonly IDictionary<Type, IDictionary<Type, List<LinkDefinition>>> _entityLinkDefinitions;
        private readonly IDictionary<Type, IDictionary<Type, List<LinkDefinition>>> _resourceLinkDefinitions;
        private readonly Type _controllerType;
        private readonly Type _modelType;

        protected ControllerMap()
        {
            _entityLinkDefinitions = new Dictionary<Type, IDictionary<Type, List<LinkDefinition>>>();
            _resourceLinkDefinitions = new Dictionary<Type, IDictionary<Type, List<LinkDefinition>>>();
            _controllerType = typeof(TController);
            _modelType = typeof(TModel);
        }

        /// <summary>
        /// The collection of <see cref="LinkDefinition" /> objects by Controller Type
        /// </summary>
        public IReadOnlyDictionary<Type, ReadOnlyDictionary<Type, ReadOnlyCollection<LinkDefinition>>>
            EntityLinkDefinitions =>
            new ReadOnlyDictionary<Type, ReadOnlyDictionary<Type, ReadOnlyCollection<LinkDefinition>>>(
                _entityLinkDefinitions.ToDictionary(outerKey => outerKey.Key, outerValue =>
                    new ReadOnlyDictionary<Type, ReadOnlyCollection<LinkDefinition>>(
                        outerValue.Value.ToDictionary(innerKey => innerKey.Key,
                            innerValue => innerValue.Value.AsReadOnly()))));

        /// <summary>
        /// The collection of <see cref="LinkDefinition" /> objects by Controller Type
        /// </summary>
        public IReadOnlyDictionary<Type, ReadOnlyDictionary<Type, ReadOnlyCollection<LinkDefinition>>>
            ResourceLinkDefinitions =>
            new ReadOnlyDictionary<Type, ReadOnlyDictionary<Type, ReadOnlyCollection<LinkDefinition>>>(
                _resourceLinkDefinitions.ToDictionary(outerKey => outerKey.Key, outerValue =>
                    new ReadOnlyDictionary<Type, ReadOnlyCollection<LinkDefinition>>(
                        outerValue.Value.ToDictionary(innerKey => innerKey.Key,
                            innerValue => innerValue.Value.AsReadOnly()))));

        /// <summary>
        /// Creates a <see cref="LinkDefinition" /> and makes it available for access in <see cref="EntityLinkDefinitions" />
        /// </summary>
        /// <param name="action">The Controller action to map to</param>
        /// <param name="valueFunc">An expression to retrieve the map of values that go into creating the resource link</param>
        /// <param name="relation">The relation this link is to the current context</param>
        /// <param name="method">The <see cref="HttpMethod" /> this route accepts</param>
        /// <param name="rootForEntity">Denotes whether this link is the root for the entity</param>
        /// <param name="rootForResource">Denotes whether this link is the root for the resource</param>
        /// <remarks>There can be only one root for a resource, so setting said property true will set all other link definitions to false</remarks>
        protected void CreateLinkDefinition(string action, Func<TModel, object> valueFunc,
            string relation, HttpMethod method, bool rootForEntity = false, bool rootForResource = false)
        {
            if (!_entityLinkDefinitions.ContainsKey(_controllerType))
                _entityLinkDefinitions.Add(_controllerType, new Dictionary<Type, List<LinkDefinition>>
                {
                    {_modelType, new List<LinkDefinition>()}
                });
            
            if (rootForResource)
                foreach (var linkDef in _entityLinkDefinitions[_controllerType][_modelType])
                    linkDef.IsRootForResource = false;
            
            if (rootForEntity)
                foreach (var linkDef in _entityLinkDefinitions[_controllerType][_modelType])
                    linkDef.IsRootForEntity = false;
            
            _entityLinkDefinitions[_controllerType][_modelType]
                .Add(new LinkDefinition(action, relation, method, valueFunc.Downcast(), rootForEntity, rootForResource));
        }

        /// <summary>
        /// Creates a <see cref="LinkDefinition" /> and makes it available for access in <see cref="ResourceLinkDefinitions" />
        /// </summary>
        /// <param name="action">The Controller action to map to</param>
        /// <param name="relation">The relation this link is to the current context</param>
        /// <param name="method">The <see cref="HttpMethod" /> this route accepts</param>
        protected void CreateLinkDefinition(string action, string relation, HttpMethod method)
        {
            if (!_resourceLinkDefinitions.ContainsKey(_controllerType))
                _resourceLinkDefinitions.Add(_controllerType, new Dictionary<Type, List<LinkDefinition>>
                {
                    {_modelType, new List<LinkDefinition>()}
                });

            _resourceLinkDefinitions[_controllerType][_modelType]
                .Add(new LinkDefinition(action, relation, method, _ => new { }));
        }
    }
}