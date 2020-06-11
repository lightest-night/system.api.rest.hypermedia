using System.Collections.Generic;

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public class LinkResource
    {
        /// <summary>
        /// The collection of Links associated with the Resource
        /// </summary>
        public List<Link> Links { get; } = new List<Link>();

    }
}