using System.Linq;
using LightestNight.System.Api.Rest.Hypermedia.Data;
using Shouldly;
using Xunit;

namespace LightestNight.System.Api.Rest.Hypermedia.Tests.Data
{
    public class DataShaperTests
    {
        private readonly DataShaper _sut;
    
        public DataShaperTests()
        {
            _sut = new DataShaper();
        }

        [Fact]
        public void ShouldShapeObjectIntoEntity()
        {
            // Arrange
            var testObject = new TestReadModel
            {
                StringProperty = "Test Property",
                IntProperty = 100
            };
            
            // Act
            var result = _sut.ShapeData(testObject);
            
            // Assert
            result.Entity.ShouldNotBeNull();
            result.Entity[nameof(TestReadModel.StringProperty)].ShouldBe(testObject.StringProperty);
            result.Entity[nameof(TestReadModel.IntProperty)].ShouldBe(testObject.IntProperty);
        }

        [Fact]
        public void ShouldOnlyIncludeRequestedFieldsObject()
        {
            // Arrange
            var testObject = new TestReadModel
            {
                StringProperty = "Test Property",
                IntProperty = 100,
                BoolProperty = true
            };
            var requestedFields = new[] {nameof(TestReadModel.StringProperty), nameof(TestReadModel.BoolProperty)};

            // Act
            var result = _sut.ShapeData(testObject, requestedFields);
            
            // Assert
            result.Entity.ContainsKey(nameof(TestReadModel.StringProperty)).ShouldBeTrue();
            result.Entity[nameof(TestReadModel.StringProperty)].ShouldBe(testObject.StringProperty);
            
            result.Entity.ContainsKey(nameof(TestReadModel.IntProperty)).ShouldBeFalse();
            
            result.Entity[nameof(TestReadModel.BoolProperty)].ShouldBe(testObject.BoolProperty);
        }

        [Fact]
        public void ShouldReturnAnEmptyEnumerableIfGivenEnumerableIsEmpty()
        {
            // Arrange
            var testObject = Enumerable.Empty<object>();
            
            // Act
            var result = _sut.ShapeData(testObject!);
            
            // Assert
            result.ShouldBeEmpty();
        }
        
        [Fact]
        public void ShouldShapeObjectsIntoEntities()
        {
            // Arrange
            var testObjects = Enumerable.Range(0, 10).Select(index => new TestReadModel
            {
                StringProperty = $"Test Property {index}",
                IntProperty = index,
                BoolProperty = index % 2 == 0
            }).ToArray();
            
            // Act
            var results = _sut.ShapeData(testObjects).ToArray();
            
            // Assert
            results.Length.ShouldBe(testObjects.Length);
            foreach (var result in results)
            {
                var testObject = testObjects.FirstOrDefault(to =>
                    to.IntProperty == (int) result.Entity[nameof(TestReadModel.IntProperty)]!);
                testObject.ShouldNotBeNull();
                
                result.Entity[nameof(TestReadModel.StringProperty)].ShouldBe(testObject.StringProperty);
                result.Entity[nameof(TestReadModel.BoolProperty)].ShouldBe(testObject.BoolProperty);
            }
        }

        [Fact]
        public void ShouldOnlyIncludeRequestedFieldsEnumerable()
        {
            // Arrange
            var testObjects = Enumerable.Range(0, 10).Select(index => new TestReadModel
            {
                StringProperty = $"Test Property {index}",
                IntProperty = index,
                BoolProperty = index % 2 == 0
            }).ToArray();
            var requestedFields = new[] {nameof(TestReadModel.StringProperty), nameof(TestReadModel.BoolProperty)};
            
            // Act
            var results = _sut.ShapeData(testObjects, requestedFields).ToArray();
            
            // Assert
            foreach (var result in results)
            {
                result.Entity.ContainsKey(nameof(TestReadModel.StringProperty)).ShouldBeTrue();
                var testObject = testObjects.FirstOrDefault(to =>
                    to.StringProperty == result.Entity[nameof(TestReadModel.StringProperty)]!.ToString());
                testObject.ShouldNotBeNull();
                
                result.Entity[nameof(TestReadModel.BoolProperty)].ShouldBe(testObject.BoolProperty);

                result.Entity.ContainsKey(nameof(TestReadModel.IntProperty)).ShouldBeFalse();
            }
        }
    }
}