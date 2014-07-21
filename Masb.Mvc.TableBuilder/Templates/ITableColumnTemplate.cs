using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents a column template.
    /// </summary>
    public interface ITableColumnTemplate
    {
        /// <summary>
        /// Accepts a visitor that need to know the generic parameters of the current <see cref="ITableColumnTemplate"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of the result returned from the visitor.</typeparam>
        /// <param name="visitor">Visitor that will then be called back, with the generic parameters.</param>
        /// <returns>The value returned from the visitor.</returns>
        TResult Accept<TResult>([NotNull] ITableColumnTemplateVisitor<TResult> visitor);
    }

    /// <summary>
    /// Represents a column template for a know row model and targeting a know property of the row model.
    /// </summary>
    /// <typeparam name="TCollectionItem">Type of the of the row model.</typeparam>
    /// <typeparam name="TSubProperty">Type of the property of the row model.</typeparam>
    public interface ITableColumnTemplate<TCollectionItem, TSubProperty> :
        ITableColumnTemplateFrom<TCollectionItem>,
        ITableColumnTemplateTo<TSubProperty>
    {
        /// <summary>
        /// Gets the expression that retrieves a <typeparamref name="TSubProperty"/> from a <typeparamref name="TCollectionItem"/>.
        /// </summary>
        [NotNull]
        Expression<Func<TCollectionItem, TSubProperty>> Expression { get; }
    }
}