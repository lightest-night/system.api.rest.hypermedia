namespace LightestNight.System.Api.Rest.Hypermedia.Data
{
    public class DataShaper
    {
        /// <summary>
        /// Creates a <see cref="ShapedEntity" /> based on the entity given and the requested fields
        /// </summary>
        /// <param name="entity">The entity to be shaped</param>
        /// <param name="requestedFields">The fields that were requested</param>
        /// <returns>An instance of <see cref="ShapedEntity" /></returns>
        public ShapedEntity ShapeData(object entity, string requestedFields = "")
        {
            return new ShapedEntity();
        }
    }
}