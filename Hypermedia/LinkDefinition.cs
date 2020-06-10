using System;
using System.Linq.Expressions;
using System.Net.Http;
using LightestNight.System.Utilities.Extensions;

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public class LinkDefinition
    {
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
        /// The map of values that go into creating the resource link
        /// </summary>
        public object Value { get; }
        
        /// <summary>
        /// Denotes whether this link is the root for the resource
        /// </summary>
        /// <remarks>The root for the resource is the link used for create or update edges</remarks>
        public bool IsRootForResource { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="LinkDefinition" />
        /// </summary>
        /// <param name="action">The Controller action to map to</param>
        /// <param name="relation">The relation this link is to the current context</param>
        /// <param name="method">The <see cref="HttpMethod" /> this route accepts</param>
        /// <param name="value">The map of values that go into creating the resource link</param>
        /// <param name="rootForResource"></param>
        public LinkDefinition(string action, string relation, HttpMethod method, object value, bool rootForResource = false)
        {
            Action = action.ThrowIfNull(nameof(action));
            Relation = relation.ThrowIfNull(nameof(relation));
            Method = method.ThrowIfNull(nameof(method));
            Value = value.ThrowIfNull(nameof(value));
            IsRootForResource = rootForResource;
        }
    }

    public class LinkDefinition<TReadModel>
    {
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
        /// An expression to retrieve the map of values that go into creating the resource link
        /// </summary>
        public Expression<Func<TReadModel, object>> ValueExpression { get; }
        
        /// <summary>
        /// Denotes whether this link is the root for the resource
        /// </summary>
        /// <remarks>The root for the resource is the link used for create or update edges</remarks>
        public bool IsRootForResource { get; set; }
        
        /// <summary>
        /// Creates an instance of <see cref="LinkDefinition" />
        /// </summary>
        /// <param name="action">The Controller action to map to</param>
        /// <param name="relation">The relation this link is to the current context</param>
        /// <param name="method">The <see cref="HttpMethod" /> this route accepts</param>
        /// <param name="valueExpression">An expression to retrieve the map of values that go into creating the resource link</param>
        /// <param name="rootForResource"></param>
        public LinkDefinition(string action, string relation, HttpMethod method, Expression<Func<TReadModel, object>> valueExpression, bool rootForResource = false)
        {
            Action = action.ThrowIfNull(nameof(action));
            Relation = relation.ThrowIfNull(nameof(relation));
            Method = method.ThrowIfNull(nameof(method));
            ValueExpression = valueExpression.ThrowIfNull(nameof(valueExpression));
            IsRootForResource = rootForResource;
        }
    }
}