using System;
using System.Linq;
using System.Net.Http;
using ExpectedObjects;
using Shouldly;
using Xunit;

namespace LightestNight.System.Api.Rest.Hypermedia.Tests
{
    public class LinkDefinitionsTestController {}
    public class LinkDefinitionsNoRootTestController {}
    
    public class LinkDefinitionsNoRootNoDefaultTestController {}
    
    public class LinkDefinitionsTestControllerMap : ControllerMap<LinkDefinitionsTestController>
    {
        public LinkDefinitionsTestControllerMap()
        {
            static object ValueFunc(TestReadModel readModel) => new {Property = readModel.StringProperty};
            CreateLinkDefinition("GET", (Func<TestReadModel, object>) ValueFunc, "self", HttpMethod.Get);
            CreateLinkDefinition("GetById", (Func<TestReadModel, object>) ValueFunc, "self", HttpMethod.Get, true);
            
            CreateLinkDefinition("GetById", "self", HttpMethod.Get);
        }
    }

    public class LinkDefinitionsNoRootTestControllerMap : ControllerMap<LinkDefinitionsNoRootTestController>
    {
        public LinkDefinitionsNoRootTestControllerMap()
        {
            static object ValueFunc(TestReadModel readModel) => new {Property = readModel.StringProperty};
            CreateLinkDefinition("GET", (Func<TestReadModel, object>) ValueFunc, "self", HttpMethod.Get);
            CreateLinkDefinition("GetById", (Func<TestReadModel, object>) ValueFunc, "self", HttpMethod.Get);
            
            CreateLinkDefinition("GetById", "self", HttpMethod.Get);
        }
    }
    
    public class LinkDefinitionsNoRootNoDefaultTestControllerMap : ControllerMap<LinkDefinitionsNoRootNoDefaultTestController>
    {
        public LinkDefinitionsNoRootNoDefaultTestControllerMap()
        {
            static object ValueFunc(TestReadModel readModel) => new {Property = readModel.StringProperty};
            CreateLinkDefinition("GET", (Func<TestReadModel, object>) ValueFunc, "self", HttpMethod.Get);
            CreateLinkDefinition("Create", (Func<TestReadModel, object>) ValueFunc, "self", HttpMethod.Post);
            
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
            var result = LinkDefinitions.GetEntityLinkDefinitions(typeof(LinkDefinitionsTestController))
                .ToArray();
            
            // Assert
            result.ShouldContain(linkDef => linkDef.Action == "GET" && linkDef.Method == HttpMethod.Get && linkDef.Relation == "self" &&
                                            expectedValue.Equals(linkDef.GetValueMap(_value)));
            result.ShouldContain(linkDef => linkDef.Action == "GetById" && linkDef.Method == HttpMethod.Get && linkDef.Relation == "self" &&
                                            expectedValue.Equals(linkDef.GetValueMap(_value)) && linkDef.IsRootForResource);
        }

        [Fact]
        public void ShouldSelectDefaultRootForResourceActionAsRootWhenNoneSet()
        {
            // Arrange
            var expectedValue = new {Property = _value.StringProperty}.ToExpectedObject();
            
            // Act
            var result = LinkDefinitions.GetEntityLinkDefinitions(typeof(LinkDefinitionsNoRootTestController))
                .ToArray();
            
            // Assert
            result.ShouldContain(linkDef =>
                linkDef.Action == Constants.DefaultRootForResourceAction && linkDef.Method == HttpMethod.Get &&
                linkDef.Relation == "self" && expectedValue.Equals(linkDef.GetValueMap(_value)) && linkDef.IsRootForResource);
            result.ShouldNotContain(linkDef =>
                linkDef.Action == Constants.DefaultRootForResourceAction && linkDef.Method == HttpMethod.Get &&
                linkDef.Relation == "self" && expectedValue.Equals(linkDef.GetValueMap(_value)) && !linkDef.IsRootForResource);
            result.ShouldContain(linkDef =>
                linkDef.Action == "GET" && linkDef.Method == HttpMethod.Get && linkDef.Relation == "self" &&
                expectedValue.Equals(linkDef.GetValueMap(_value)) && !linkDef.IsRootForResource);
        }

        [Fact]
        public void ShouldGetNoRootEntityLinkDefinitions()
        {
            // Act
            var result = LinkDefinitions.GetEntityLinkDefinitions(typeof(LinkDefinitionsNoRootNoDefaultTestController));
            
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
            var result = LinkDefinitions.GetEntityLinkDefinitions(typeof(LinkDefinitionsTestController))
                .ToArray();
            
            // Assert
            foreach (var testObject in testObjects)
            {
                var expectedValue = new {Property = testObject.StringProperty}.ToExpectedObject();
                result.ShouldContain(linkDef =>
                    linkDef.Action == "GET" && linkDef.Method == HttpMethod.Get && linkDef.Relation == "self" &&
                    expectedValue.Equals(linkDef.GetValueMap(testObject)) && !linkDef.IsRootForResource);
                result.ShouldContain(linkDef =>
                    linkDef.Action == "GetById" && linkDef.Method == HttpMethod.Get && linkDef.Relation == "self" &&
                    expectedValue.Equals(linkDef.GetValueMap(testObject)) && linkDef.IsRootForResource);
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