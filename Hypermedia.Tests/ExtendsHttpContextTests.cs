using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Shouldly;
using Xunit;

namespace LightestNight.System.Api.Rest.Hypermedia.Tests
{
    public class ExtendsHttpContextTests
    {
        [Fact]
        public void ShouldReturnTrueForHateoas()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Items.Add(Constants.AcceptHeaderMediaType, new MediaTypeHeaderValue("application/vnd.test.hateoas+json"));
            
            // Act
            var result = httpContext.IsHateoas();
            
            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void ShouldReturnFalseForHateoas()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Items.Add(Constants.AcceptHeaderMediaType, new MediaTypeHeaderValue("application/json"));
            
            // Act
            var result = httpContext.IsHateoas();
            
            // Assert
            result.ShouldBeFalse();
        }
    }
}