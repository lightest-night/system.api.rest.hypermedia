using System;
using ExpectedObjects;
using Shouldly;
using Xunit;

namespace LightestNight.System.Api.Rest.Hypermedia.Tests
{
    public class ExtendsFuncWithModelTests
    {
        [Fact]
        public void ShouldDownCastFuncWithModel()
        {
            // Arrange
            var funcWithModel = new Func<TestReadModel, object>(readModel => new {Property = readModel.StringProperty});
            
            // Act
            var result = funcWithModel.Downcast();
            
            // Assert
            result.ShouldBeOfType(typeof(Func<object, object>));
        }

        [Fact]
        public void ShouldOutputValueCorrectly()
        {
            // Arrange
            var funcWithModel = new Func<TestReadModel, object>(model => new {Property = model.StringProperty});
            const string propertyValue = "Test Property";
            var readModel = new TestReadModel
            {
                StringProperty = propertyValue
            };
            var expectedObject = new {Property = propertyValue}.ToExpectedObject();
            
            // Act
            var downcastFunc = funcWithModel.Downcast();
            var result = downcastFunc(readModel);

            // Assert
            result.ShouldBe(expectedObject);
        }
    }
}