﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LightestNight.System.Api.Rest.Hypermedia.Data
{
    public class Entity : DynamicObject, IXmlSerializable, IDictionary<string, object?>
    {
        private readonly IDictionary<string, object?> _expando;

        public int Count => _expando.Count;
        public bool IsReadOnly => _expando.IsReadOnly;
        
        public object? this[string key]
        {
            get => _expando[key];
            set => _expando[key] = value;
        }

        public ICollection<string> Keys => _expando.Keys;
        public ICollection<object?> Values => _expando.Values;

        public Entity()
        {
            _expando = new ExpandoObject();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
        
        public bool Contains(KeyValuePair<string, object?> item)
            => _expando.Contains(item);
        
        public bool Remove(KeyValuePair<string, object?> item)
            => _expando.Remove(item);
        
        public bool ContainsKey(string key)
            => _expando.ContainsKey(key);

        public bool Remove(string key)
            => _expando.Remove(key);

        public bool TryAdd(string key, object value)
            => _expando.TryAdd(key, value);

        public bool TryGetValue(string key, out object value)
            => _expando.TryGetValue(key, out value!);
        
        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
            => _expando.GetEnumerator();

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            if (!_expando.TryGetValue(binder.Name, out var value)) 
                return base.TryGetMember(binder, out result);
            
            result = value;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            _expando[binder.Name] = value;
            return true;
        }
        
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in _expando.Keys)
            {
                var value = _expando[key];
                WriteXmlElement(key, value, writer);   
            }
        }

        public void Add(KeyValuePair<string, object?> item)
        {
            _expando.Add(item);
        }

        public void Clear()
        {
            _expando.Clear();
        }

        public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
        {
            _expando.CopyTo(array, arrayIndex);
        }

        public void Add(string key, object? value)
        {
            _expando.Add(key, value);
        }

        private static void WriteXmlElement(string key, object? value, XmlWriter writer)
        {
            if (value == null)
                return;

            writer.WriteStartElement(key);

            void WriteElement(Link link)
            {
                writer.WriteStartElement(nameof(Link));
                WriteXmlElement(nameof(link.Href), link.Href, writer);
                WriteXmlElement(nameof(link.Method), link.Method, writer);
                WriteXmlElement(nameof(link.Rel), link.Rel, writer);
                writer.WriteEndElement();
            }

            switch (value)
            {
                case Link[] array:
                    foreach (var link in array)
                        WriteElement(link);
                    break;
                
                case List<Link> list:
                    foreach (var link in list)
                        WriteElement(link);
                    break;
                
                case IEnumerable<Link> enumerable:
                    foreach (var link in enumerable as Link[] ?? enumerable.ToArray())
                        WriteElement(link);
                    break;
                
                default:
                    writer.WriteString(value.ToString());
                    break;
            }

            writer.WriteEndElement();
        }
    }
}