﻿using System;
using System.Net.Http;

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public class LinkDefinition
    {
        private readonly Func<object, object> _valueFunction;
        
        /// <summary>
        /// The Controller action to map to
        /// </summary>
        public string Action { get; }
        
        /// <summary>
        /// The relation this link is to the current context
        /// </summary>
        public string Relation { get; }
        
        /// <summary>
        /// The <see cref="HttpMethod" /> this route accepts
        /// </summary>
        public HttpMethod Method { get; }
        
        /// <summary>
        /// Denotes whether this link is the root for the resource
        /// </summary>
        /// <remarks>The root for the resource is the link used for create or update edges</remarks>
        public bool IsRootForResource { get; set; }
        
        /// <summary>
        /// Denotes whether this link is the root for the entity, and will be within the root list
        /// </summary>
        public bool IsRootForEntity { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="LinkDefinition" />
        /// </summary>
        /// <param name="action">The Controller action to map to</param>
        /// <param name="relation">The relation this link is to the current context</param>
        /// <param name="method">The <see cref="HttpMethod" /> this route accepts</param>
        /// <param name="valueFunc">A function to retrieve the map of values that go into creating the resource link</param>
        /// <param name="rootForEntity">Denotes whether this is the root for the entity</param>
        /// <param name="rootForResource">Denotes whether this is the root for the resource</param>
        public LinkDefinition(string action, string relation, HttpMethod method, Func<object, object> valueFunc,
            bool rootForEntity = false, bool rootForResource = false)
        {
            Action = action;
            Relation = relation;
            Method = method;
            IsRootForEntity = rootForEntity;
            IsRootForResource = rootForResource;

            _valueFunction = valueFunc;
        }

        /// <summary>
        /// Executes the Value Function to generate the value map
        /// </summary>
        /// <param name="value">The model to use to generate the value map</param>
        /// <typeparam name="TModel">Type of model</typeparam>
        /// <returns>An object using the value function. This is then used to populate the link</returns>
        public object GetValueMap<TModel>(TModel value) where TModel : notnull
            => _valueFunction(value);
    }
}