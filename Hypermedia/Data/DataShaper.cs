using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightestNight.System.Utilities.Extensions;
#pragma warning disable 1591

namespace LightestNight.System.Api.Rest.Hypermedia.Data
{
    public class DataShaper
    {
        private PropertyInfo[] _properties = Array.Empty<PropertyInfo>();
        
        /// <summary>
        /// Creates a <see cref="ShapedEntity" /> based on the entity given and the requested fields
        /// </summary>
        /// <param name="entity">The entity to be shaped</param>
        /// <param name="requestedFields">The fields that were requested, if none requested then all returned</param>
        /// <returns>An instance of <see cref="ShapedEntity" /></returns>
        public ShapedEntity ShapeData(object entity, params string[] requestedFields)
        {
            if (entity == null)
                return new ShapedEntity();

            PopulateProperties(entity.GetType());
            var requestedProperties = GetRequestedProperties(requestedFields);
            return FetchData(entity, requestedProperties);
        }

        /// <summary>
        /// Creates an enumerable of <see cref="ShapedEntity" /> objects based on the enumerable of entities given and the requested fields
        /// </summary>
        /// <param name="entities">The entities to be shaped</param>
        /// <param name="requestedFields">The fields that were requested, if none requested then all returned</param>
        /// <returns>An enumerable of <see cref="ShapedEntity" /> instances</returns>
        public IEnumerable<ShapedEntity> ShapeData(IEnumerable<object> entities, params string[] requestedFields)
        {
            var entityArray = entities as object[] ?? entities.ToArray();
            if (entityArray.IsNullOrEmpty())
                return Enumerable.Empty<ShapedEntity>();

            PopulateProperties(entityArray[0].GetType());
            var requestedProperties = GetRequestedProperties(requestedFields);
            return entityArray.Select(entity => FetchData(entity, requestedProperties));
        }

        private void PopulateProperties(IReflect type)
        {
            _properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        private IEnumerable<PropertyInfo> GetRequestedProperties(string[] requestedFields)
        {
            if (requestedFields.IsNullOrEmpty())
                return _properties;
            
            return requestedFields
                .Select(field => _properties.FirstOrDefault(property =>
                    property.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                .Where(property => property != null);
        }

        private static ShapedEntity FetchData(object entity, IEnumerable<PropertyInfo> requestedProperties)
        {
            var shapedObject = new ShapedEntity();

            foreach (var property in requestedProperties)
            {
                var objectPropertyValue = property.GetValue(entity);
                if (objectPropertyValue == null)
                    continue;
                
                shapedObject.Entity.TryAdd(property.Name, objectPropertyValue);
            }

            shapedObject.Original = entity;

            return shapedObject;
        }
    }
}