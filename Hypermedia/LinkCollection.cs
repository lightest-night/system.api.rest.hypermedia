using System.Collections.Generic;
using System.Linq;
#pragma warning disable 1591

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public class LinkCollection<TResource> : LinkResource
    {
        /// <summary>
        /// A Collection of <typeparamref name="TResource" /> objects
        /// </summary>
        public List<TResource> Value { get; }

        /// <summary>
        /// Creates a new instance of <see cref="LinkCollection{TResource}" />
        /// </summary>
        public LinkCollection()
        {
            Value = new List<TResource>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="LinkCollection{TResource}" /> with the given enumerable
        /// </summary>
        /// <param name="value">An enumerable of <typeparamref name="TResource" /> objects</param>
        public LinkCollection(IEnumerable<TResource> value)
        {
            Value = value as List<TResource> ?? value.ToList();
        }
    }
}