using System.Net.Http;

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public class Link
    {
        /// <summary>
        /// The target URI of this Link
        /// </summary>
        public string Href { get; set; }
        
        /// <summary>
        /// Link relation type, describes how the current context is related to the target resource
        /// </summary>
        public string Rel { get; set; }
        
        /// <summary>
        /// The HTTP Method the current context accepts
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="Link" />
        /// </summary>
        public Link()
        {
            Href = string.Empty;
            Rel = string.Empty;
            Method = string.Empty;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Link" />
        /// </summary>
        /// <param name="href">The target URI of this Link</param>
        /// <param name="relation">Link relation type, describes how the current context is related to the target resource</param>
        /// <param name="method">The HTTP Method the current context accepts</param>
        public Link(string href, string relation, HttpMethod method)
        {
            Href = href.ToLowerInvariant();
            Rel = relation.ToLowerInvariant();
            Method = method.ToString().ToUpperInvariant();
        }

    }
}