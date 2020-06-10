﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public abstract class ControllerMap<TController, TReadModel>
    {
        private readonly IDictionary<Type, List<LinkDefinition<TReadModel>>> _entityLinkDefinitions;
        private readonly IDictionary<Type, List<LinkDefinition>> _resourceLinkDefinitions;
        private readonly Type _controllerType;

        protected ControllerMap()
        {
            _entityLinkDefinitions = new Dictionary<Type, List<LinkDefinition<TReadModel>>>();
            _resourceLinkDefinitions = new Dictionary<Type, List<LinkDefinition>>();
            _controllerType = typeof(TController);
        }

        /// <summary>
        /// The collection of <see cref="LinkDefinition{TReadModel}" /> objects by <typeparam name="TController"></typeparam>
        /// </summary>
        public IDictionary<Type, LinkDefinition<TReadModel>[]> EntityLinkDefinitions
            => _entityLinkDefinitions.ToDictionary(key => key.Key, value => value.Value.ToArray());

        /// <summary>
        /// The collection of <see cref="LinkDefinition" /> objects by <typeparam name="TController"></typeparam>
        /// </summary>
        public IDictionary<Type, LinkDefinition[]> ResourceLinkDefinitions
            => _resourceLinkDefinitions.ToDictionary(key => key.Key, value => value.Value.ToArray());

        /// <summary>
        /// Creates a <see cref="LinkDefinition{TReadModel}" /> and makes it available for access in <see cref="EntityLinkDefinitions" />
        /// </summary>
        /// <param name="action">The Controller action to map to</param>
        /// <param name="valueExpression">An expression to retrieve the map of values that go into creating the resource link</param>
        /// <param name="relation">The relation this link is to the current context</param>
        /// <param name="method">The <see cref="HttpMethod" /> this route accepts</param>
        /// <param name="rootForResource">Denotes whether this link is the root for the resource</param>
        /// <remarks>There can be only one root for a resource, so setting said property true will set all other link definitions to false</remarks>
        protected void CreateLinkDefinition(string action, Expression<Func<TReadModel, object>> valueExpression,
            string relation, HttpMethod method, bool rootForResource = false)
        {
            if (!_entityLinkDefinitions.ContainsKey(_controllerType))
                _entityLinkDefinitions.Add(_controllerType, new List<LinkDefinition<TReadModel>>());
            
            if (rootForResource)
                foreach (var linkDef in _entityLinkDefinitions[_controllerType])
                    linkDef.IsRootForResource = false;
            
            _entityLinkDefinitions[_controllerType].Add(new LinkDefinition<TReadModel>(action, relation, method, valueExpression, rootForResource));
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
                _resourceLinkDefinitions.Add(_controllerType, new List<LinkDefinition>());
            
            _resourceLinkDefinitions[_controllerType].Add(new LinkDefinition(action, relation, method, new {}));
        }
    }
}