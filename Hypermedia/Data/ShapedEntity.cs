namespace LightestNight.System.Api.Rest.Hypermedia.Data
{
    public class ShapedEntity
    {
        /// <summary>
        /// The <see cref="Entity" /> to shape
        /// </summary>
        public Entity Entity { get; }
        
        /// <summary>
        /// The original object that was then shaped
        /// </summary>
        #nullable enable
        public object? Original { get; set; }
        #nullable restore

        /// <summary>
        /// Creates a new instance of <see cref="ShapedEntity" />
        /// </summary>
        public ShapedEntity()
        {
            Entity = new Entity();
        }
    }
}