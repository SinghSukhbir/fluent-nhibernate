using System;

namespace FluentNHibernate.Mapping
{
    public interface IOneToManyPart : ICollectionRelationship
    {
        CollectionCascadeExpression<IOneToManyPart> Cascade { get; }
        IOneToManyPart Inverse();
        IOneToManyPart LazyLoad();

        /// <summary>
        /// Sets a custom collection type
        /// </summary>
        IOneToManyPart CollectionType<TCollection>();

        /// <summary>
        /// Sets a custom collection type
        /// </summary>
        IOneToManyPart CollectionType(Type type);

        /// <summary>
        /// Sets a custom collection type
        /// </summary>
        IOneToManyPart CollectionType(string type);

        /// <summary>
        /// Inverts the next boolean
        /// </summary>
        IOneToManyPart Not { get; }
        string ColumnName { get; }
        Type ParentType { get; }
        IOneToManyPart WithKeyColumn(string name);
    }
}
