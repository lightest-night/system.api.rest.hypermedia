using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace LightestNight.System.Api.Rest.Hypermedia.Tests
{
    public class LinkCollectionTests
    {
        [Fact]
        public void ShouldCreateNewLinkCollectionWithEmptyValue()
        {
            // Act
            var result = new LinkCollection<TestReadModel>();
            
            // Assert
            result.Value.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldCreateNewLinkCollectionWithGivenListValue()
        {
            // Arrange
            var value = new List<TestReadModel>
            {
                new TestReadModel()
            };
            
            // Act
            var result = new LinkCollection<TestReadModel>(value);
            
            // Assert
            result.Value.ShouldBe(value);
        }

        [Fact]
        public void ShouldCreateNewLinkCollectionWithGivenArrayValue()
        {
            // Arrange
            var value = new[]
            {
                new TestReadModel()
            };
            
            // Act
            var result = new LinkCollection<TestReadModel>(value);
            
            // Assert
            result.Value.ShouldBe(value);
        }
    }
}