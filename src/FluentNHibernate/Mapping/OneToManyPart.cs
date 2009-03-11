﻿using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System;

namespace FluentNHibernate.Mapping
{
    public class OneToManyPart<PARENT, CHILD> : ToManyBase<OneToManyPart<PARENT, CHILD>, PARENT, CHILD>, IOneToManyPart, IAccessStrategy<OneToManyPart<PARENT, CHILD>> 
    {
        private readonly IList<string> _columnNames = new List<string>();

        public OneToManyPart(PropertyInfo property)
            : this(property, property.PropertyType)
        {}

        public OneToManyPart(MethodInfo method)
            : this(method, method.ReturnType)
        {}

        protected OneToManyPart(MemberInfo member, Type collectionType)
            : base(member, collectionType)
        {
            _properties.Store("name", member.Name);          
        }

        public override void Write(XmlElement classElement, IMappingVisitor visitor)
        {
            XmlElement collectionElement = WriteCollectionElement(classElement);
            WriteKeyElement(visitor, collectionElement);

            if (_indexMapping != null)
                WriteIndexElement(collectionElement);

            WriteMappingTypeElement(visitor, collectionElement);
        }

        private XmlElement WriteCollectionElement(XmlElement classElement)
        {
            if (_collectionType == "array")
                _properties.Remove("collection-type");

            XmlElement collectionElement = classElement.AddElement(_collectionType)
                .WithProperties(_properties);

            if (!string.IsNullOrEmpty(TableName))
                collectionElement.SetAttribute("table", TableName);

            return collectionElement;
        }

        private void WriteMappingTypeElement(IMappingVisitor visitor, XmlElement collectionElement)
        {
            if (_elementMapping != null) 
            {
                var elementElement = collectionElement.AddElement("element");                
                _elementMapping.WriteAttributesToIndexElement(elementElement);                
            } 
            else if (_componentMapping == null) 
            {
                // standard one-to-many element
                collectionElement.AddElement("one-to-many")
                    .SetAttribute("class", typeof(CHILD).AssemblyQualifiedName);
            }
            else
            {
                // specified a component, so output that instead
                _componentMapping.Write(collectionElement, visitor);
            }
        }

        private void WriteIndexElement(XmlElement collectionElement)
        {
            var indexElement = collectionElement.AddElement("index");
            _indexMapping.WriteAttributesToIndexElement(indexElement);
        }

        private void WriteKeyElement(IMappingVisitor visitor, XmlElement collectionElement)
        {
            if (_columnNames.Count == 1)
                _keyProperties.Store("column", _columnNames[0]);

            var key = collectionElement.AddElement("key")
                .WithProperties(_keyProperties);

            if (_columnNames.Count <= 1) return;

            foreach (var columnName in _columnNames)
            {
                key.AddElement("column")
                    .WithAtt("name", columnName);
            }
        }

        /// <summary>
        /// Set an attribute on the xml element produced by this one-to-many mapping.
        /// </summary>
        /// <param name="name">Attribute name</param>
        /// <param name="value">Attribute value</param>
        public override void SetAttribute(string name, string value)
        {
            _properties.Store(name, value);
        }

        public override void SetAttributes(Attributes atts)
        {
            foreach (var key in atts.Keys)
            {
                SetAttribute(key, atts[key]);
            }
        }

        public override int Level
        {
            get { return 3; }
        }

        public override PartPosition Position
        {
            get { return PartPosition.Anywhere; }
        }

        public FetchTypeExpression<OneToManyPart<PARENT, CHILD>> FetchType
        {
            get
            {
                return new FetchTypeExpression<OneToManyPart<PARENT, CHILD>>(this, _properties);
            }
        }

        public OneToManyPart<PARENT, CHILD> WithKeyColumns(params string[] columnNames)
        {
            foreach (var columnName in columnNames)
            {
                WithKeyColumn(columnName);
            }

            return this;
        }

        public Type ParentType
        {
            get { return typeof(PARENT); }
        }
       
        public string ColumnName
        {
            get { return _columnNames.Count > 0 ? _columnNames[0] : null; }
        }

        public OneToManyPart<PARENT, CHILD> WithKeyColumn(string columnName)
        {
            _columnNames.Add(columnName);
            return this;
        }

        #region Explicit IOneToManyPart Implementation

        CollectionCascadeExpression<IOneToManyPart> IOneToManyPart.Cascade
        {
            get { return new CollectionCascadeExpression<IOneToManyPart>(this); }
        }

        IOneToManyPart IOneToManyPart.Inverse()
        {
            return this.Inverse();
        }

        IOneToManyPart IOneToManyPart.LazyLoad()
        {
            return this.LazyLoad();
        }

        IOneToManyPart IOneToManyPart.Not
        {
            get { return this.Not; }
        }

        /// <summary>
        /// Sets a custom collection type
        /// </summary>
        IOneToManyPart IOneToManyPart.CollectionType<TCollection>()
        {
            return this.CollectionType<TCollection>();
        }

        /// <summary>
        /// Sets a custom collection type
        /// </summary>
        IOneToManyPart IOneToManyPart.CollectionType(Type type)
        {
            return this.CollectionType(type);
        }

        /// <summary>
        /// Sets a custom collection type
        /// </summary>
        IOneToManyPart IOneToManyPart.CollectionType(string type)
        {
            return this.CollectionType(type);
        }

        IOneToManyPart IOneToManyPart.WithKeyColumn(string name)
        {
            return WithKeyColumn(name);
        }

        #endregion
    }
}
