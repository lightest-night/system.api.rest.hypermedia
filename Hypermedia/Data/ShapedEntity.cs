#pragma warning disable 1591

namespace LightestNight.System.Api.Rest.Hypermedia.Data
{
    public class ShapedEntity
    {
        /// <summary>
        /// The <see cref="Entity" /> to shape
        /// </summary>
        public Entity Entity { get; }
        
        /// <summary>
        /// Creates a new instance of <see cref="ShapedEntity" />
        /// </summary>
        public ShapedEntity()
        {
            Entity = new Entity();
        }
    }
}