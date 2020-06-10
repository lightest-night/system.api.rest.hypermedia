using LightestNight.System.Api.Rest.Hypermedia.Data;
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
        }
    }
}