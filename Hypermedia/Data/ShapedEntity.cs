namespace LightestNight.System.Api.Rest.Hypermedia.Data
{
    public class ShapedEntity
    {
        /// <summary>
        /// The <see cref="Entity" /> to shape
        /// </summary>
        public Entity Entity { get; set; }
        
        /// <summary>
        /// The original object that was then shaped
        /// </summary>
        public object? Original { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="ShapedEntity" />
        /// </summary>
        public ShapedEntity()
        {
            Entity = new Entity();
        }
    }
}