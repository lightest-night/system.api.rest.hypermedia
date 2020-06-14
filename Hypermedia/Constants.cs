#pragma warning disable 1591

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public static class Constants
    {
        /// <summary>
        /// If no definition is set as the root, then if one exists with this value, set that as the root
        /// </summary>
        public const string DefaultRootForResourceAction = "GetById";

        /// <summary>
        /// The HttpContext item key for the Accept Header Media Type
        /// </summary>
        public const string AcceptHeaderMediaType = nameof(AcceptHeaderMediaType);

        /// <summary>
        /// The identifier to determine if a request is accepting the HATEOAS response
        /// </summary>
        public const string HateoasIdentifier = "hateoas";

        /// <summary>
        /// The key to store links under
        /// </summary>
        public const string LinksKey = "Links";

        /// <summary>
        /// The JSON Media Type
        /// </summary>
        public const string JsonMediaType = "application/json";
    }
}