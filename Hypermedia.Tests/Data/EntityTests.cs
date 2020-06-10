using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml;
using LightestNight.System.Api.Rest.Hypermedia.Data;
using Shouldly;
using Xunit;

namespace LightestNight.System.Api.Rest.Hypermedia.Tests.Data
{
    public class EntityTests
    {
        private const string Key = nameof(Key);
        private const string Value = nameof(Value);

        private const string LinksXml =
            @"﻿<?xml version=""1.0"" encoding=""utf-8""?><Links><Link><Href>href</Href><Method>GET</Method><Rel>relation</Rel></Link><Link><Href>href1</Href><Method>DELETE</Method><Rel>relation</Rel></Link></Links>";

        private readonly List<Link> _links = new List<Link>
        {
            new Link("Href", "Relation", HttpMethod.Get),
            new Link("Href1", "Relation", HttpMethod.Delete)
        };
        
        private readonly Entity _sut;

        public EntityTests()
        {
            _sut = new Entity();
        }

        [Fact]
        public void ShouldAddValueToExpando()
        {
            // Act
            _sut.Add(Key, Value);
            
            // Assert
            _sut.ContainsKey(Key).ShouldBeTrue();
        }

        [Fact]
        public void ShouldGetProperXml()
        {
            // Arrange
            _sut.Add(Key, Value);
            using var xmlStream = new MemoryStream();
            using var writer = XmlWriter.Create(xmlStream);
            _sut.WriteXml(writer);
            writer.Flush();
            xmlStream.Position = 0;
            
            // Act
            var xml = Encoding.UTF8.GetString(xmlStream.ToArray());
            
            // Assert
            xml.ShouldContain("<Key>Value</Key>");
        }

        [Fact]
        public void ShouldSerialiseLinksToXmlProperly()
        {
            // Arrange
            _sut.Add("Links", _links);
            using var xmlStream = new MemoryStream();
            using var writer = XmlWriter.Create(xmlStream);
            _sut.WriteXml(writer);
            writer.Flush();
            xmlStream.Position = 0;
            
            // Act
            var xml = Encoding.UTF8.GetString(xmlStream.ToArray());
            
            // Assert
            xml.ShouldBe(LinksXml);
        }
    }
}