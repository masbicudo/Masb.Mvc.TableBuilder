using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents a column template for a know row model (that is the item of the collection).
    /// </summary>
    /// <typeparam name="TCollectionItem">Type of the of the row model.</typeparam>
    public interface ITableColumnTemplateFrom<TCollectionItem> :
        ITableColumnTemplate
    {
        /// <summary>
        /// Accepts a visitor that need to know the generic parameters of the current <see cref="ITableColumnTemplate"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of the result returned from the visitor.</typeparam>
        /// <param name="visitor">Visitor that will then be called back, with the generic parameters.</param>
        /// <returns>The value returned from the visitor.</returns>
        TResult Accept<TResult>([NotNull] ITableColumnTemplateFromVisitor<TCollectionItem, TResult> visitor);
    }
}