using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using ExpectedObjects;
using Shouldly;
using Xunit;

namespace LightestNight.System.Api.Rest.Hypermedia.Tests
{
    public class LinkDefinitionsTestController {}
    public class LinkDefinitionsNoRootTestController {}
    
    public class LinkDefinitionsNoRootNoDefaultTestController {}
    
    public class LinkDefinitionsTestControllerMap : ControllerMap<LinkDefinitionsTestController, TestReadModel>
    {
        public LinkDefinitionsTestControllerMap()
        {
            Expression<Func<TestReadModel, object>> valueExpression = readModel => new {Property = readModel.StringProperty};
            CreateLinkDefinition("GET", valueExpression, "self", HttpMethod.Get);
            CreateLinkDefinition("GetById", valueExpression, "self", HttpMethod.Get, true);
            
            CreateLinkDefinition("GetById", "self", HttpMethod.Get);
        }
    }

    public class LinkDefinitionsNoRootTestControllerMap : ControllerMap<LinkDefinitionsNoRootTestController, TestReadModel>
    {
        public LinkDefinitionsNoRootTestControllerMap()
        {
            Expression<Func<TestReadModel, object>> valueExpression = readModel => new {Property = readModel.StringProperty};
            CreateLinkDefinition("GET", valueExpression, "self", HttpMethod.Get);
            CreateLinkDefinition("GetById", valueExpression, "self", HttpMethod.Get);
            
            CreateLinkDefinition("GetById", "self", HttpMethod.Get);
        }
    }
    
    public class LinkDefinitionsNoRootNoDefaultTestControllerMap : ControllerMap<LinkDefinitionsNoRootNoDefaultTestController, TestReadModel>
    {
        public LinkDefinitionsNoRootNoDefaultTestControllerMap()
        {
            Expression<Func<TestReadModel, object>> valueExpression = readModel => new {Property = readModel.StringProperty};
            CreateLinkDefinition("GET", valueExpression, "self", HttpMethod.Get);
            CreateLinkDefinition("Create", valueExpression, "self", HttpMethod.Post);
            
            CreateLinkDefinition("GetById", "self", HttpMethod.Get);
        }
    }
    
    public class LinkDefinitionsTests
    {
        private readonly TestReadModel _value = new TestReadModel {StringProperty = "String Property"};
        
        [Fact]
        public void ShouldGetEntityLinkDefinitionsForSingularObject()
        {
            // Arrange
            var expectedValue = new {Property = _value.StringProperty}.ToExpectedObject();
            
            // Act
            var result = LinkDefinitions.GetEntityLinkDefinitions(typeof(LinkDefinitionsTestController), _value)
                .ToArray();
            
            // Assert
            result.ShouldContain(linkDef =>
                linkDef.Action == "GET" && linkDef.Method == HttpMethod.Get && linkDef.Relation == "self" &&
                expectedValue.Equals(linkDef.Value) && !linkDef.IsRootForResource);
            result.ShouldContain(linkDef =>
                linkDef.Action == "GetById" && linkDef.Method == HttpMethod.Get && linkDef.Relation == "self" &&
                expectedValue.Equals(linkDef.Value) && linkDef.IsRootForResource);
        }

        [Fact]
        public void ShouldSelectDefaultRootForResourceActionAsRootWhenNoneSet()
        {
            // Arrange
            var expectedValue = new {Property = _value.StringProperty}.ToExpectedObject();
            
            // Act
            var result = LinkDefinitions.GetEntityLinkDefinitions(typeof(LinkDefinitionsNoRootTestController), _value)
                .ToArray();
            
            // Assert
            result.ShouldContain(linkDef =>
                linkDef.Action == Constants.DefaultRootForResourceAction && linkDef.Method == HttpMethod.Get &&
                linkDef.Relation == "self" && expectedValue.Equals(linkDef.Value) && linkDef.IsRootForResource);
            result.ShouldNotContain(linkDef =>
                linkDef.Action == Constants.DefaultRootForResourceAction && linkDef.Method == HttpMethod.Get &&
                linkDef.Relation == "self" && expectedValue.Equals(linkDef.Value) && !linkDef.IsRootForResource);
            result.ShouldContain(linkDef =>
                linkDef.Action == "GET" && linkDef.Method == HttpMethod.Get && linkDef.Relation == "self" &&
                expectedValue.Equals(linkDef.Value) && !linkDef.IsRootForResource);
        }

        [Fact]
        public void ShouldGetNoRootEntityLinkDefinitions()
        {
            // Act
            var result = LinkDefinitions.GetEntityLinkDefinitions(typeof(LinkDefinitionsNoRootNoDefaultTestController), _value);
            
            // Assert
            result.ShouldNotContain(linkDef => linkDef.IsRootForResource);
        }

        [Fact]
        public void ShouldGetEntityLinkDefinitionsForEnumerable()
        {
            // Arrange
            var testObjects = new[]
            {
                _value,
                new TestReadModel {StringProperty = "String Property Multiple 1"},
                new TestReadModel {StringProperty = "String Property Multiple 2"},
                new TestReadModel {StringProperty = "String Property Multiple 3"},
            };
            
            // Act
            var result = LinkDefinitions.GetEntityLinkDefinitions(typeof(LinkDefinitionsTestController), testObjects)
                .ToArray();
            
            // Assert
            foreach (var testObject in testObjects)
            {
                var expectedValue = new {Property = testObject.StringProperty}.ToExpectedObject();
                result.ShouldContain(linkDef =>
                    linkDef.Action == "GET" && linkDef.Method == HttpMethod.Get && linkDef.Relation == "self" &&
                    expectedValue.Equals(linkDef.Value) && !linkDef.IsRootForResource);
                result.ShouldContain(linkDef =>
                    linkDef.Action == "GetById" && linkDef.Method == HttpMethod.Get && linkDef.Relation == "self" &&
                    expectedValue.Equals(linkDef.Value) && linkDef.IsRootForResource);
            }
        }

        [Fact]
        public void ShouldGetResourceLinkDefinitions()
        {
            // Act
            var result = LinkDefinitions.GetResourceLinkDefinitions(typeof(LinkDefinitionsTestController)).ToArray();

            // Assert
            result.ShouldContain(linkDef => linkDef.Action == "GetById" && linkDef.Method == HttpMethod.Get && linkDef.Relation == "self");
        }
    }
}