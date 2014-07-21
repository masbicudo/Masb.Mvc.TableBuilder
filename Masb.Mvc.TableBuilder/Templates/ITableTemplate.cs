using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents a table template.
    /// </summary>
    public interface ITableTemplate
    {
        /// <summary>
        /// Gets the columns defined by this table template.
        /// </summary>
        IEnumerable<ITableColumnTemplate> Columns { get; }

        /// <summary>
        /// Accepts a visitor that need to know the generic parameters of the current <see cref="ITableTemplate"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of the result returned from the visitor.</typeparam>
        /// <param name="visitor">Visitor that will then be called back, with the generic parameters.</param>
        /// <returns>The value returned from the visitor.</returns>
        TResult Accept<TResult>(ITableTemplateVisitor<TResult> visitor);
    }

    /// <summary>
    /// Represents a table template for a known root model.
    /// </summary>
    /// <typeparam name="TModel">Type of the root model that contains the items collection.</typeparam>
    public interface ITableTemplate<TModel> :
        ITableTemplate
    {
        /// <summary>
        /// Accepts a visitor that need to know the generic parameters of the current <see cref="ITableTemplate"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of the result returned from the visitor.</typeparam>
        /// <param name="visitor">Visitor that will then be called back, with the generic parameters.</param>
        /// <returns>The value returned from the visitor.</returns>
        TResult Accept<TResult>(ITableTemplateVisitor<TModel, TResult> visitor);
    }

    /// <summary>
    /// Represents a table template for a known root model and collection of items.
    /// </summary>
    /// <typeparam name="TModel">Type of the root model that contains the items collection.</typeparam>
    /// <typeparam name="TCollectionItem">Type of the items in the target items collection.</typeparam>
    public interface ITableTemplate<TModel, TCollectionItem> :
        ITableTemplate<TModel>,
        ITableTemplateTo<TCollectionItem>
    {
        /// <summary>
        /// Gets the expression that retrieves a list of <typeparamref name="TCollectionItem"/> from a <typeparamref name="TModel"/>.
        /// </summary>
        Expression<Func<TModel, IList<TCollectionItem>>> Expression { get; }
    }
}