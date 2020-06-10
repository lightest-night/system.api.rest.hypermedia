using System;
using System.Linq.Expressions;
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
        private readonly object _value = new {Value = "Value"};

        private readonly Expression<Func<TestReadModel, object>> _valueExpression =
            readModel => new {Property = readModel.StringProperty};
        
        [Fact]
        public void ShouldCreateLinkDefinitionValueObjectWithoutRoot()
        {
            // Act
            var result = new LinkDefinition(Action, Relation, _method, _value);
            
            // Assert
            result.Action.ShouldBe(Action);
            result.Relation.ShouldBe(Relation);
            result.Method.ShouldBe(_method);
            result.Value.ShouldBe(_value);
            result.IsRootForResource.ShouldBeFalse();
        }

        [Fact]
        public void ShouldCreateLinkDefinitionValueExpressionWithoutRoot()
        {
            // Act
            var result = new LinkDefinition<TestReadModel>(Action, Relation, _method, _valueExpression);
            
            // Assert
            result.Action.ShouldBe(Action);
            result.Relation.ShouldBe(Relation);
            result.Method.ShouldBe(_method);
            result.ValueExpression.ShouldBe(_valueExpression);
            result.IsRootForResource.ShouldBeFalse();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldCreateLinkDefinitionValueObjectWithRoot(bool isRoot)
        {
            // Act
            var result = new LinkDefinition(Action, Relation, _method, _value, isRoot);
            
            // Assert
            result.Action.ShouldBe(Action);
            result.Relation.ShouldBe(Relation);
            result.Method.ShouldBe(_method);
            result.Value.ShouldBe(_value);
            result.IsRootForResource.ShouldBe(isRoot);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldCreateLinkDefinitionValueExpressionWithRoot(bool isRoot)
        {
            // Act
            var result = new LinkDefinition<TestReadModel>(Action, Relation, _method, _valueExpression, isRoot);
            
            // Assert
            result.Action.ShouldBe(Action);
            result.Relation.ShouldBe(Relation);
            result.Method.ShouldBe(_method);
            result.ValueExpression.ShouldBe(_valueExpression);
            result.IsRootForResource.ShouldBe(isRoot);
        }

        [Fact]
        public void ShouldResolveValueExpressionCorrectly()
        {
            // Arrange
            var linkDefinition = new LinkDefinition<TestReadModel>(Action, Relation, _method, _valueExpression);
            var readModel = new TestReadModel
            {
                StringProperty = "Test Property"
            };
            var expectedObject = new {Property = readModel.StringProperty}.ToExpectedObject();
            
            // Act
            var result = linkDefinition.ValueExpression.Compile()(readModel);
            
            // Assert
            result.ShouldBe(expectedObject);
        }
    }
}