using System.Linq;
using System.Net.Http;
using Shouldly;
using Xunit;

namespace LightestNight.System.Api.Rest.Hypermedia.Tests
{
    public class LinkTests
    {
        [Fact]
        public void ShouldCreateEmptyLink()
        {
            // Act
            var result = new Link();
            
            // Assert
            result.Href.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldCreateLink()
        {
            // Arrange
            const string href = nameof(href);
            const string rel = nameof(rel);
            var method = HttpMethod.Get;
            
            // Act
            var result = new Link(href, rel, method);
            
            // Assert
            result.Href.ShouldBe(href);
            result.Rel.ShouldBe(rel);
            result.Method.ShouldBe(method.ToString());
        }

        [Theory]
        [InlineData("HREF", "RELATION")]
        [InlineData("href", "relation")]
        [InlineData("HrEf", "ReLaTiOn")]
        public void HrefAndRelationShouldBeLowerCase(string href, string relation)
        {
            // Act
            var result = new Link(href, relation, HttpMethod.Get);
            
            // Assert
            result.Href.All(char.IsLower).ShouldBeTrue();
            result.Rel.All(char.IsLower).ShouldBeTrue();
        }

        [Fact]
        public void MethodShouldBeUpperCase()
        {
            // Act
            var result = new Link("", "", HttpMethod.Get);
            
            // Assert
            result.Method.All(char.IsUpper).ShouldBeTrue();
        }
    }
}