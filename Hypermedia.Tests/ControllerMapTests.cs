using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using Shouldly;
using Xunit;

namespace LightestNight.System.Api.Rest.Hypermedia.Tests
{
    public class TestController {}
    
    public class TestControllerMap : ControllerMap<TestController, TestReadModel>
    {
        public TestControllerMap()
        {
            Expression<Func<TestReadModel, object>> valueExpression = readModel => new {Property = readModel.StringProperty};
            CreateLinkDefinition("GET", valueExpression, "self", HttpMethod.Get);
            CreateLinkDefinition("GetById", valueExpression, "self", HttpMethod.Get, true);
            CreateLinkDefinition("GetByIdAgain", valueExpression, "self", HttpMethod.Get, true);

            CreateLinkDefinition("GetById", "self", HttpMethod.Get);
        }
    }
    
    public class ControllerMapTests
    {
        private readonly TestControllerMap _sut;
    
        public ControllerMapTests()
        {
            _sut = new TestControllerMap();
        }
    
        [Fact]
        public void ShouldCreateEntityLinkDefinition()
        {
            // Act
            var controllerLinkDefinitions = _sut.EntityLinkDefinitions[typeof(TestController)];
            var linkDefinition = controllerLinkDefinitions.FirstOrDefault(linkDef => linkDef.Action == "GET");
            
            // Assert
            linkDefinition.ShouldNotBeNull();
        }

        [Fact]
        public void ShouldCreateEntityLinkDefinitionsWithOnlyOneRoot()
        {
            // Act
            var controllerLinkDefinitions = _sut.EntityLinkDefinitions[typeof(TestController)];
            var rootDefinitions = controllerLinkDefinitions.Where(linkDef => linkDef.IsRootForResource).ToArray();
            
            // Assert
            rootDefinitions.Length.ShouldBe(1);
        }

        [Fact]
        public void ShouldCreateResourceLinkDefinition()
        {
            // Act
            var controllerLinkDefinitions = _sut.ResourceLinkDefinitions[typeof(TestController)];
            var linkDefinition = controllerLinkDefinitions.FirstOrDefault(linkDef => linkDef.Action == "GetById");
            
            // Assert
            linkDefinition.ShouldNotBeNull();
        }
    }
}