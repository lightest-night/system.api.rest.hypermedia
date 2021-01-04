using System;
using System.Net.Http;
using ExpectedObjects;
using Shouldly;
using Xunit;

namespace LightestNight.System.Api.Rest.Hypermedia.Tests
{
    public class LinkDefinitionTests
    {
        private const string Action = "Action";
        private const string Relation = "Relation";
        private readonly HttpMethod _method = HttpMethod.Get;

        private static readonly Func<TestReadModel, object> ValueFunc =
            readModel => new {Property = readModel.StringProperty};

        private static readonly Func<object, object> ValueAccessor = ValueFunc.Downcast();
        
        [Fact]
        public void ShouldCreateLinkDefinitionValueExpressionWithoutRoot()
        {
            // Act
            var result = new LinkDefinition(Action, Relation, _method, ValueAccessor);
            
            // Assert
            result.Action.ShouldBe(Action);
            result.Relation.ShouldBe(Relation);
            result.Method.ShouldBe(_method);
            result.IsRootForResource.ShouldBeFalse();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldCreateLinkDefinitionValueExpressionWithRootForResource(bool isRoot)
        {
            // Act
            var result = new LinkDefinition(Action, Relation, _method, ValueAccessor, rootForResource: isRoot);
            
            // Assert
            result.Action.ShouldBe(Action);
            result.Relation.ShouldBe(Relation);
            result.Method.ShouldBe(_method);
            result.IsRootForResource.ShouldBe(isRoot);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldCreateLinkDefinitionValueExpressionWithRootForEntity(bool isRoot)
        {
            // Act
            var result = new LinkDefinition(Action, Relation, _method, ValueAccessor, isRoot);
            
            // Assert
            result.Action.ShouldBe(Action);
            result.Relation.ShouldBe(Relation);
            result.Method.ShouldBe(_method);
            result.IsRootForEntity.ShouldBe(isRoot);
        }

        [Fact]
        public void ShouldResolveValueExpressionCorrectly()
        {
            // Arrange
            var linkDefinition = new LinkDefinition(Action, Relation, _method, ValueAccessor);
            var readModel = new TestReadModel
            {
                StringProperty = "Test Property"
            };
            var expectedObject = new {Property = readModel.StringProperty}.ToExpectedObject();
            
            // Act
            var result = linkDefinition.GetValueMap(readModel);
            
            // Assert
            result.ShouldBe(expectedObject);
        }
    }
}